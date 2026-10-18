namespace InvoiceApp.AI.Agents;

/// <summary>
/// Core contract representing an autonomous, specialized agent in the multi-agent swarm architecture.
/// </summary>
/// <remarks>
/// Every specialized agent (e.g., Invoice Assistant, Customer Operations, Analytics, Documentation)
/// implements this interface. Agents possess specialized system instructions, bound domain tools,
/// and can execute either via live LLM reasoning (<see cref="Microsoft.Extensions.AI.IChatClient"/>)
/// or offline local simulation fallback.
/// </remarks>
public interface IBaseAgent
{
    /// <summary>
    /// Gets the unique, human-readable display name of the agent (e.g. "Invoice Assistant Agent").
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets a functional description of the agent's responsibilities and capabilities,
    /// used by the coordinator and router to evaluate task suitability.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Executes the agent's reasoning cycle against the provided user prompt, conversational history,
    /// and contextual page state.
    /// </summary>
    /// <param name="request">The agent input request containing prompt, conversation ID, and page context.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The generated agent response containing message content, duration, token metrics, and metadata.</returns>
    Task<AgentResponse> ExecuteAsync(
        AgentRequest request,
        CancellationToken cancellationToken = default);
}
