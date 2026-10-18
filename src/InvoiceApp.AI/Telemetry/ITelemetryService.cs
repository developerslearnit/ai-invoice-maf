namespace InvoiceApp.AI.Telemetry;

/// <summary>
/// Defines telemetry tracking contracts for observing autonomous agent performance, execution latency, and token consumption.
/// </summary>
public interface ITelemetryService
{
    /// <summary>
    /// Records the operational metrics for an executed agent invocation.
    /// </summary>
    /// <param name="agentName">The friendly or technical identifier of the executing agent (e.g., "Invoice Assistant Agent").</param>
    /// <param name="elapsedMilliseconds">The wall-clock duration in milliseconds spent executing the agent reasoning and tool invocation.</param>
    /// <param name="totalTokens">The estimated or actual number of prompt and completion tokens utilized during LLM generation.</param>
    void TrackAgentExecution(string agentName, long elapsedMilliseconds, int totalTokens);
}
