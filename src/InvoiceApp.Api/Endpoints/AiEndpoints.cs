using InvoiceApp.AI.Coordinator;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceApp.Api.Endpoints;

/// <summary>
/// Minimal API endpoint mappings for the AI Assistant subsystem.
/// </summary>
/// <remarks>
/// Exposes <c>POST /api/ai/chat</c> to accept natural language prompts from client applications,
/// dispatch them to <see cref="IAgentCoordinator"/>, and return structured responses with
/// agent attribution, latency, confidence, and contextual suggestion prompts.
/// </remarks>
public static class AiEndpoints
{
    /// <summary>
    /// Maps AI chat Minimal API routes into the application pipeline.
    /// </summary>
    /// <param name="app">The route builder to extend.</param>
    /// <returns>The route builder for chaining.</returns>
    public static IEndpointRouteBuilder MapAiEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/ai")
            .WithTags("AI Assistant");

        group.MapPost("/chat", async (
            [FromBody] AiChatApiRequest request,
            IAgentCoordinator coordinator,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return Results.BadRequest(new { error = "Message cannot be empty." });
            }

            var conversationId = string.IsNullOrWhiteSpace(request.ConversationId)
                ? Guid.NewGuid().ToString("N")[..8]
                : request.ConversationId;

            var result = await coordinator.ProcessAsync(conversationId, request.Message, request.Context, cancellationToken);

            var suggestions = GenerateSuggestions(result.AgentResponse.AgentName);

            var response = new AiChatApiResponse(
                Message: result.AgentResponse.Content,
                ConversationId: conversationId,
                AgentName: result.AgentResponse.AgentName,
                IsSuccess: result.AgentResponse.IsSuccess,
                Suggestions: suggestions,
                ConfidenceScore: result.Evaluation.ConfidenceScore,
                DurationMs: result.AgentResponse.DurationMs
            );

            return Results.Ok(response);
        })
        .WithName("SendAiMessage")
        .WithSummary("Send prompt to Microsoft Agent Framework coordinator")
        .WithDescription("Routes the prompt to specialized autonomous agents (Invoice Assistant, Customer Operations, Analytics, Documentation) and executes domain tools.");

        return app;
    }

    /// <summary>
    /// Generates contextual follow-up prompt suggestions tailored to the specific agent that answered.
    /// </summary>
    /// <param name="agentName">The name of the responding agent.</param>
    /// <returns>A list of suggested follow-up prompts.</returns>
    private static IReadOnlyList<string> GenerateSuggestions(string agentName)
    {
        return agentName switch
        {
            "Invoice Operations Agent" => ["Show overdue invoices", "Mark INV-000001 as Paid", "Summarize invoice activity"],
            "Customer Operations Agent" => ["List all customers", "Create customer Apex Corp", "Show overdue invoices"],
            "Billing Analytics Agent" => ["Which customer owes the most?", "Show overdue invoices", "Summarize overall business"],
            "Platform Documentation Agent" => ["How do I create an invoice?", "How do I create a customer?", "What are the status transition rules?"],
            _ => ["Show overdue invoices", "Summarize invoice activity", "Which customer owes the most?"]
        };
    }
}

/// <summary>
/// Request payload sent to the AI chat endpoint.
/// </summary>
/// <param name="Message">The user message or action prompt.</param>
/// <param name="ConversationId">Optional conversation session identifier.</param>
/// <param name="Context">Optional client-side state context (active invoice ID, customer ID, active page).</param>
public sealed record AiChatApiRequest(
    string Message,
    string? ConversationId = null,
    IReadOnlyDictionary<string, object>? Context = null
);

/// <summary>
/// Response payload returned by the AI chat endpoint.
/// </summary>
/// <param name="Message">The generated textual response or tool output.</param>
/// <param name="ConversationId">The conversation session identifier.</param>
/// <param name="AgentName">The friendly name of the executing autonomous agent.</param>
/// <param name="IsSuccess">Indicates whether execution completed successfully.</param>
/// <param name="Suggestions">Contextual follow-up suggestions for the user interface.</param>
/// <param name="ConfidenceScore">Normalized confidence score between 0.0 and 1.0.</param>
/// <param name="DurationMs">Total execution latency in milliseconds.</param>
public sealed record AiChatApiResponse(
    string Message,
    string ConversationId,
    string AgentName,
    bool IsSuccess,
    IReadOnlyList<string> Suggestions,
    double ConfidenceScore,
    long DurationMs
);
