namespace InvoiceApp.AI.Agents;

/// <summary>
/// Encapsulates the execution parameters passed to an autonomous agent.
/// </summary>
/// <param name="Prompt">The raw user query or natural language command.</param>
/// <param name="ConversationId">Optional conversation session identifier for multi-turn conversational context.</param>
/// <param name="UserId">Optional user identifier for auditing or role-based permission checks.</param>
/// <param name="Context">Optional contextual dictionary extracted from active client screen (e.g. active invoice ID, customer name).</param>
public sealed record AgentRequest(
    string Prompt,
    string? ConversationId = null,
    string? UserId = null,
    IReadOnlyDictionary<string, object>? Context = null
);

/// <summary>
/// Represents the result produced by an autonomous agent after reasoning and domain tool calling.
/// </summary>
/// <param name="Content">The natural language reply or tool execution confirmation message.</param>
/// <param name="AgentName">The name of the agent that produced this response.</param>
/// <param name="IsSuccess">Indicates whether the agent executed its workflow successfully without unhandled errors.</param>
/// <param name="TotalTokens">The estimated or actual number of prompt and completion tokens consumed.</param>
/// <param name="DurationMs">The total execution latency in milliseconds.</param>
/// <param name="Metadata">Optional additional metadata produced by domain tools or reasoning steps.</param>
public sealed record AgentResponse(
    string Content,
    string AgentName,
    bool IsSuccess = true,
    int TotalTokens = 0,
    long DurationMs = 0,
    IReadOnlyDictionary<string, object>? Metadata = null
)
{
    /// <summary>
    /// Creates a successful agent response.
    /// </summary>
    /// <param name="content">The message content produced by the agent.</param>
    /// <param name="agentName">The name of the responding agent.</param>
    /// <param name="totalTokens">Total token count consumed.</param>
    /// <param name="durationMs">Execution duration in milliseconds.</param>
    /// <param name="metadata">Optional dictionary containing tool execution outputs or entity references.</param>
    /// <returns>A configured <see cref="AgentResponse"/> marked as successful.</returns>
    public static AgentResponse Success(
        string content,
        string agentName,
        int totalTokens = 0,
        long durationMs = 0,
        IReadOnlyDictionary<string, object>? metadata = null) =>
        new(content, agentName, true, totalTokens, durationMs, metadata);

    /// <summary>
    /// Creates a failure agent response.
    /// </summary>
    /// <param name="errorMessage">The error description explaining why execution could not proceed.</param>
    /// <param name="agentName">The name of the responding agent.</param>
    /// <param name="durationMs">Execution duration in milliseconds.</param>
    /// <returns>A configured <see cref="AgentResponse"/> marked as failed.</returns>
    public static AgentResponse Failure(
        string errorMessage,
        string agentName,
        long durationMs = 0) =>
        new(errorMessage, agentName, false, 0, durationMs);
}
