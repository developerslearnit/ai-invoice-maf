using Microsoft.Extensions.Logging;

namespace InvoiceApp.AI.Telemetry;

/// <summary>
/// Default implementation of <see cref="ITelemetryService"/> that logs agent execution telemetry
/// to the structured logging pipeline.
/// </summary>
/// <param name="logger">The structured logger instance.</param>
/// <remarks>
/// Emits standardized <c>METRIC: AgentExecution</c> log events suitable for ingestion into
/// Application Insights, OpenTelemetry collectors, or log analytical systems.
/// </remarks>
public sealed class TelemetryService(ILogger<TelemetryService> logger) : ITelemetryService
{
    /// <inheritdoc />
    public void TrackAgentExecution(string agentName, long elapsedMilliseconds, int totalTokens)
    {
        logger.LogInformation(
            "METRIC: AgentExecution [Agent={AgentName}] [DurationMs={ElapsedMs}] [Tokens={Tokens}]",
            agentName, elapsedMilliseconds, totalTokens);
    }
}
