using System.Diagnostics;
using InvoiceApp.AI.Common;
using InvoiceApp.AI.Telemetry;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace InvoiceApp.AI.Agents;

/// <summary>
/// Abstract foundational base class for all specialized autonomous agents in InvoiceAI.
/// </summary>
/// <remarks>
/// <para>
/// Implements <see cref="IBaseAgent"/> using Microsoft Agent Framework patterns and <see cref="Microsoft.Extensions.AI.IChatClient"/>.
/// Handles:
/// </para>
/// <list type="bullet">
///   <item><description>System prompt instruction injection.</description></item>
///   <item><description>Dynamic binding of <see cref="AITool"/> functions for LLM autonomous tool calling.</description></item>
///   <item><description>Automatic failover to offline <see cref="SimulationMode"/> when cloud keys are unavailable or unconfigured.</description></item>
///   <item><description>Comprehensive execution duration and token usage instrumentation via <see cref="ITelemetryService"/>.</description></item>
/// </list>
/// </remarks>
public abstract class BaseAgent : IBaseAgent
{
    private readonly IChatClient? _chatClient;
    private readonly ITelemetryService _telemetry;
    private readonly ILogger _logger;
    private readonly SimulationMode _simulationMode;
    private readonly List<AITool> _tools = [];

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public string Description { get; }

    /// <summary>
    /// Gets the system prompt instructions defining the persona, behavioral boundaries, and workflow guidelines for this agent.
    /// </summary>
    public string Instructions { get; }

    /// <summary>
    /// Gets the collection of <see cref="AITool"/> instances registered and available for function calling by this agent.
    /// </summary>
    public IReadOnlyList<AITool> Tools => _tools.AsReadOnly();

    /// <summary>
    /// Initializes a new instance of <see cref="BaseAgent"/>.
    /// </summary>
    /// <param name="chatClient">The underlying <see cref="IChatClient"/> (null when operating in offline simulation mode).</param>
    /// <param name="instructions">The system instructions injected as the system persona.</param>
    /// <param name="name">The friendly name of the agent.</param>
    /// <param name="description">The functional description of the agent.</param>
    /// <param name="tools">Optional list of domain tools that this agent can autonomously call.</param>
    /// <param name="telemetry">The telemetry service for operational tracking.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="simulationMode">The simulation mode configuration.</param>
    protected BaseAgent(
        IChatClient? chatClient,
        string instructions,
        string name,
        string description,
        IEnumerable<AITool>? tools,
        ITelemetryService telemetry,
        ILogger logger,
        SimulationMode simulationMode)
    {
        _chatClient = chatClient;
        Instructions = instructions;
        Name = name;
        Description = description;
        _telemetry = telemetry;
        _logger = logger;
        _simulationMode = simulationMode;

        if (tools is not null)
        {
            _tools.AddRange(tools);
        }
    }

    /// <inheritdoc />
    public virtual async Task<AgentResponse> ExecuteAsync(
        AgentRequest request,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("Agent '{AgentName}' executing request with prompt length: {Length}", Name, request.Prompt.Length);

        // Simulation / Offline Mode Fallback
        if (_simulationMode.IsEnabled || _chatClient is null)
        {
            _logger.LogInformation("Agent '{AgentName}' executing in SIMULATION mode.", Name);
            await Task.Delay(150, cancellationToken); // Realistic simulated inference delay
            stopwatch.Stop();

            var simulatedText = await GenerateSimulatedResponseAsync(request, cancellationToken);
            _telemetry.TrackAgentExecution(Name, stopwatch.ElapsedMilliseconds, 0);

            return AgentResponse.Success(simulatedText, Name, 0, stopwatch.ElapsedMilliseconds);
        }

        try
        {
            var chatOptions = new ChatOptions
            {
                Tools = _tools,
                Temperature = 0.2f
            };

            var messages = new List<ChatMessage>
            {
                new(ChatRole.System, Instructions),
                new(ChatRole.User, request.Prompt)
            };

            var response = await _chatClient.GetResponseAsync(messages, chatOptions, cancellationToken);
            stopwatch.Stop();

            int tokens = (int)(response.Usage?.TotalTokenCount ?? 0);
            _telemetry.TrackAgentExecution(Name, stopwatch.ElapsedMilliseconds, tokens);

            return AgentResponse.Success(
                content: response.Text ?? string.Empty,
                agentName: Name,
                totalTokens: tokens,
                durationMs: stopwatch.ElapsedMilliseconds
            );
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error occurred during execution of agent '{AgentName}'", Name);
            return AgentResponse.Failure(ex.Message, Name, stopwatch.ElapsedMilliseconds);
        }
    }

    /// <summary>
    /// Generates a contextual, functional response when the agent is running in offline <see cref="SimulationMode"/>.
    /// </summary>
    /// <param name="request">The incoming execution request.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A formatted simulated response string, often calling the agent's real domain tools directly.</returns>
    protected virtual Task<string> GenerateSimulatedResponseAsync(AgentRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult($"[SIMULATION MODE - {Name}]: Successfully received request '{request.Prompt}'. In live mode with Azure OpenAI credentials configured, this agent autonomously invokes bound tools to execute domain actions and queries.");
    }
}
