using InvoiceApp.AI.Agents;
using InvoiceApp.AI.Evaluations;
using InvoiceApp.AI.Memory;
using InvoiceApp.AI.Telemetry;
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
///   <item><description><b>Semantic Routing:</b> Classifies user intent and routes to the best-matching specialized agent (<see cref="DocumentationAgent"/>, <see cref="CustomerOperationsAgent"/>, <see cref="AnalyticsAgent"/>, or <see cref="InvoiceAssistantAgent"/>).</description></item>
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
public sealed class AgentCoordinator(
    IEnumerable<IBaseAgent> agents,
    IAgentMemoryService memoryService,
    IAgentEvaluationService evaluationService,
    ITelemetryService telemetryService,
    ILogger<AgentCoordinator> logger) : IAgentCoordinator
{
    private readonly List<IBaseAgent> _agents = agents.ToList();

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

        // 2. Select the Best Matching Specialized Agent
        var targetAgent = RouteToAgent(userPrompt);
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
    /// Inspects the natural language prompt and routes execution to the most qualified specialized agent in the swarm.
    /// </summary>
    /// <param name="userPrompt">The user prompt to evaluate.</param>
    /// <returns>The designated <see cref="IBaseAgent"/> instance.</returns>
    private IBaseAgent RouteToAgent(string userPrompt)
    {
        var lower = userPrompt.ToLowerInvariant();

        // 1. Documentation & Rules
        if (lower.Contains("how to") || lower.Contains("how do i") || lower.Contains("how can i") || lower.Contains("guide") || lower.Contains("documentation") || lower.Contains("rule") || lower.Contains("can i delete"))
        {
            return _agents.OfType<DocumentationAgent>().FirstOrDefault() ?? _agents.First();
        }

        // 2. Customer Operations
        if (lower.Contains("customer") || lower.Contains("client") || lower.Contains("onboard"))
        {
            if (lower.Contains("create customer") || lower.Contains("add customer") || lower.Contains("update customer") || lower.Contains("find customer"))
            {
                return _agents.OfType<CustomerOperationsAgent>().FirstOrDefault() ?? _agents.First();
            }
        }

        // 3. Financial Analytics & High-level KPIs
        if (lower.Contains("kpi") || lower.Contains("revenue") || lower.Contains("summary") || lower.Contains("activity") || lower.Contains("analytics") || lower.Contains("who owes"))
        {
            return _agents.OfType<AnalyticsAgent>().FirstOrDefault() ?? _agents.First();
        }

        // 4. Invoice Operations (Create, Update Status, Overdue, Search by number, Delete)
        return _agents.OfType<InvoiceAssistantAgent>().FirstOrDefault() ?? _agents.First();
    }
}
