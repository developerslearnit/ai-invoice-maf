using System.Text.RegularExpressions;
using InvoiceApp.AI.Agents;
using InvoiceApp.AI.Common;
using InvoiceApp.AI.Evaluations;
using InvoiceApp.AI.Memory;
using InvoiceApp.AI.Telemetry;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace InvoiceApp.AI.Coordinator;

/// <summary>
/// Defines the orchestrator contract for coordinating multi-agent swarm operations,
/// context resolution, memory enrichment, and response evaluations.
/// </summary>
public interface IAgentCoordinator
{
    /// <summary>
    /// Coordinates the full end-to-end lifecycle of an AI request: memory enrichment,
    /// dynamic agent routing, execution, response evaluation, and telemetry tracking.
    /// </summary>
    /// <param name="conversationId">The session identifier for tracking conversational context across turns.</param>
    /// <param name="userPrompt">The raw user message or instruction.</param>
    /// <param name="context">Optional client UI context (e.g. current page, active invoice ID, active customer ID).</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="CoordinatorResult"/> containing the executing agent's response and evaluation metrics.</returns>
    Task<CoordinatorResult> ProcessAsync(
        string conversationId,
        string userPrompt,
        IReadOnlyDictionary<string, object>? context = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Encapsulates the output of a multi-agent coordination process.
/// </summary>
/// <param name="AgentResponse">The response produced by the selected autonomous agent.</param>
/// <param name="Evaluation">The evaluation result certifying policy compliance and confidence score.</param>
public sealed record CoordinatorResult(
    AgentResponse AgentResponse,
    AgentEvaluationResult Evaluation
);

/// <summary>
/// Supervisor orchestrator implementation managing autonomous agent dispatching,
/// conversational memory injection, and post-generation evaluation.
/// </summary>
/// <remarks>
/// <para>
/// Operates as the central routing hub:
/// </para>
/// <list type="number">
///   <item><description><b>Memory Enrichment:</b> Fetches sliding window history from <see cref="IAgentMemoryService"/>.</description></item>
///   <item><description><b>Semantic Routing:</b> Classifies user intent using an LLM supervisor (with sticky conversation-history fallback) and routes to the best-matching specialized agent (<see cref="DocumentationAgent"/>, <see cref="CustomerOperationsAgent"/>, <see cref="AnalyticsAgent"/>, or <see cref="InvoiceAssistantAgent"/>).</description></item>
///   <item><description><b>Execution:</b> Dispatches prompt and contextual screen state to the chosen agent.</description></item>
///   <item><description><b>Evaluation:</b> Validates output via <see cref="IAgentEvaluationService"/>.</description></item>
///   <item><description><b>Persistence:</b> Stores user and assistant turns back to memory.</description></item>
/// </list>
/// </remarks>
/// <param name="agents">The registered collection of specialized autonomous agents implementing <see cref="IBaseAgent"/>.</param>
/// <param name="memoryService">The conversational memory service.</param>
/// <param name="evaluationService">The response quality and compliance evaluation service.</param>
/// <param name="telemetryService">The telemetry logging service.</param>
/// <param name="logger">Structured logger instance.</param>
/// <param name="chatClient">Optional chat client used for live LLM semantic intent classification.</param>
/// <param name="simulationMode">Simulation mode configuration governing offline fallback behavior.</param>
public sealed class AgentCoordinator(
    IEnumerable<IBaseAgent> agents,
    IAgentMemoryService memoryService,
    IAgentEvaluationService evaluationService,
    ITelemetryService telemetryService,
    ILogger<AgentCoordinator> logger,
    IChatClient? chatClient = null,
    SimulationMode? simulationMode = null) : IAgentCoordinator
{
    private readonly List<IBaseAgent> _agents = agents.ToList();
    private readonly IChatClient? _chatClient = chatClient;
    private readonly SimulationMode _simulationMode = simulationMode ?? new SimulationMode(true);

    /// <inheritdoc />
    public async Task<CoordinatorResult> ProcessAsync(
        string conversationId,
        string userPrompt,
        IReadOnlyDictionary<string, object>? context = null,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Coordinator processing request for conversation {ConversationId}", conversationId);

        // 1. Retrieve Conversation Context from Memory
        var history = await memoryService.GetConversationHistoryAsync(conversationId, cancellationToken);
        var enrichedPrompt = string.IsNullOrWhiteSpace(history) ? userPrompt : $"{history}\nUser: {userPrompt}";

        // 2. Select the Best Matching Specialized Agent via Semantic LLM Router or Sticky Session
        var targetAgent = await RouteToAgentAsync(userPrompt, history, cancellationToken);
        logger.LogInformation("Coordinator routed prompt to '{AgentName}'", targetAgent.Name);

        // 3. Execute Selected Agent with AgentRequest
        var agentRequest = new AgentRequest(enrichedPrompt, conversationId, Context: context);
        var agentResponse = await targetAgent.ExecuteAsync(agentRequest, cancellationToken);

        // 4. Evaluate Response Quality & Policy Adherence
        var evaluation = await evaluationService.EvaluateResponseAsync(userPrompt, agentResponse.Content, cancellationToken);

        // 5. Store Interaction in Memory & Track Telemetry
        await memoryService.AppendMessageAsync(conversationId, "User", userPrompt, cancellationToken);
        await memoryService.AppendMessageAsync(conversationId, agentResponse.AgentName, agentResponse.Content, cancellationToken);
        telemetryService.TrackAgentExecution("Coordinator", agentResponse.DurationMs, agentResponse.TotalTokens);

        return new CoordinatorResult(
            AgentResponse: agentResponse,
            Evaluation: evaluation
        );
    }

    /// <summary>
    /// Inspects the natural language prompt and conversation history, delegating execution to the most qualified specialized agent in the swarm.
    /// When online, leverages an LLM classifier; when offline or in simulation mode, uses sticky conversation history heuristics.
    /// </summary>
    /// <param name="userPrompt">The user prompt to evaluate.</param>
    /// <param name="history">The accumulated conversation history.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The designated <see cref="IBaseAgent"/> instance.</returns>
    private async Task<IBaseAgent> RouteToAgentAsync(string userPrompt, string history, CancellationToken cancellationToken)
    {
        // 1. Live LLM Semantic Routing when chat client is configured and not in simulation
        if (!_simulationMode.IsEnabled && _chatClient is not null)
        {
            try
            {
                var classifiedAgent = await ClassifyIntentWithLlmAsync(userPrompt, history, cancellationToken);
                if (classifiedAgent is not null)
                {
                    logger.LogInformation("LLM Intent Router selected agent '{AgentName}'", classifiedAgent.Name);
                    return classifiedAgent;
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "LLM intent routing encountered an error. Falling back to sticky heuristic router.");
            }
        }

        // 2. Deterministic Sticky Session & Heuristic Routing (Offline / Simulation / Fallback)
        return RouteHeuristicallyWithHistory(userPrompt, history);
    }

    /// <summary>
    /// Invokes the LLM to classify user intent across registered agents taking recent conversational history into account.
    /// </summary>
    private async Task<IBaseAgent?> ClassifyIntentWithLlmAsync(string userPrompt, string history, CancellationToken cancellationToken)
    {
        var agentCatalog = string.Join("\n", _agents.Select(a => $"- \"{a.Name}\": {a.Description}"));

        var systemPrompt = 
            $"""
            You are the Supervisor Intent Router for an enterprise invoicing system.
            Your job is to select the single best agent to handle the user's latest message.

            Available Agents:
            {agentCatalog}

            Routing Rules:
            1. Multi-turn continuation: If previous conversation shows an agent asked a question, requested missing parameters (name, email, invoice line items, dates, etc.), or asked for confirmation, and the user's latest message is providing those details or answering (e.g. names, emails, amounts, 'yes'/'no', 'proceed'), SELECT THAT SAME AGENT.
            2. Intent switch: If the user explicitly asks for a different domain task (e.g., asking for documentation, creating an invoice while previously on customer onboarding, or asking for revenue analytics), select the new agent matching that domain.
            3. Response Format: Reply ONLY with the exact name of the selected agent. Do not provide any markdown, quotes, prefix, or explanation.
            """;

        var userMessage = string.IsNullOrWhiteSpace(history)
            ? $"User Message: {userPrompt}"
            : $"Recent Conversation History:\n{history}\n\nLatest User Message: {userPrompt}";

        var options = new ChatOptions
        {
            Temperature = 0.0f,
            MaxOutputTokens = 30
        };

        var response = await _chatClient!.GetResponseAsync(
            [new(ChatRole.System, systemPrompt), new(ChatRole.User, userMessage)],
            options,
            cancellationToken);

        var selectedName = response.Text?.Trim().Trim('"', '\'', '`', '.');
        if (string.IsNullOrWhiteSpace(selectedName))
        {
            return null;
        }

        return _agents.FirstOrDefault(a => a.Name.Equals(selectedName, StringComparison.OrdinalIgnoreCase))
            ?? _agents.FirstOrDefault(a => a.Name.Contains(selectedName, StringComparison.OrdinalIgnoreCase))
            ?? _agents.FirstOrDefault(a => selectedName.Contains(a.Name, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Deterministic fallback for simulation mode or API unavailability:
    /// Combines last-speaker affinity with explicit intent detection.
    /// </summary>
    private IBaseAgent RouteHeuristicallyWithHistory(string userPrompt, string history)
    {
        var lower = userPrompt.ToLowerInvariant();

        // 1. Explicit high-confidence intent switches
        if (lower.Contains("how to") || lower.Contains("how do i") || lower.Contains("how can i") || lower.Contains("guide") || lower.Contains("documentation") || lower.Contains("rule") || lower.Contains("can i delete"))
        {
            return _agents.OfType<DocumentationAgent>().FirstOrDefault() ?? _agents.First();
        }

        if (lower.Contains("kpi") || lower.Contains("revenue") || lower.Contains("summary") || lower.Contains("activity") || lower.Contains("analytics") || lower.Contains("who owes"))
        {
            return _agents.OfType<AnalyticsAgent>().FirstOrDefault() ?? _agents.First();
        }

        if (lower.Contains("create customer") || lower.Contains("add customer") || lower.Contains("update customer") || lower.Contains("find customer"))
        {
            return _agents.OfType<CustomerOperationsAgent>().FirstOrDefault() ?? _agents.First();
        }

        if (lower.Contains("create invoice") || lower.Contains("new invoice") || lower.Contains("mark as paid") || lower.Contains("cancel invoice"))
        {
            return _agents.OfType<InvoiceAssistantAgent>().FirstOrDefault() ?? _agents.First();
        }

        // 2. Multi-turn continuation: Inspect the last assistant message in conversation history
        if (!string.IsNullOrWhiteSpace(history))
        {
            var match = Regex.Match(history, @"\[([^\]]+)\]:[^\n]*$", RegexOptions.RightToLeft);
            if (match.Success)
            {
                var lastSpeaker = match.Groups[1].Value.Trim();
                var stickyAgent = _agents.FirstOrDefault(a => a.Name.Equals(lastSpeaker, StringComparison.OrdinalIgnoreCase));
                if (stickyAgent is not null)
                {
                    logger.LogInformation("Maintaining sticky routing to '{AgentName}' based on conversation history", stickyAgent.Name);
                    return stickyAgent;
                }
            }
        }

        // 3. Fallback domain keyword matching
        if (lower.Contains("customer") || lower.Contains("client") || lower.Contains("onboard"))
        {
            return _agents.OfType<CustomerOperationsAgent>().FirstOrDefault() ?? _agents.First();
        }

        // 4. Default fallback
        return _agents.OfType<InvoiceAssistantAgent>().FirstOrDefault() ?? _agents.First();
    }
}
