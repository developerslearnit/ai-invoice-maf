using InvoiceApp.Web.Models.Api;
using InvoiceApp.Web.Models.ViewModels;

namespace InvoiceApp.Web.Services.ApiClients;

/// <summary>
/// Typed HTTP client contract for communicating with the AI Assistant endpoints on the backend API.
/// </summary>
public interface IAiApiClient
{
    /// <summary>
    /// Sends a natural language chat request to <c>/api/ai/chat</c> on the backend API.
    /// </summary>
    /// <param name="request">The chat request containing prompt, conversation ID, and screen context.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>An <see cref="ApiResponse{T}"/> wrapping the resulting <see cref="AiChatApiResponseModel"/>.</returns>
    Task<ApiResponse<AiChatApiResponseModel>> SendChatMessageAsync(AiChatRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// API response model received from the backend AI endpoint.
/// </summary>
/// <param name="Message">The generated markdown message.</param>
/// <param name="ConversationId">The conversation session identifier.</param>
/// <param name="AgentName">The name of the executing agent.</param>
/// <param name="IsSuccess">Whether the execution succeeded.</param>
/// <param name="Suggestions">Suggested follow-up prompt chips.</param>
/// <param name="ConfidenceScore">Normalized confidence score.</param>
/// <param name="DurationMs">Total latency in milliseconds.</param>
public sealed record AiChatApiResponseModel(
    string Message,
    string ConversationId,
    string AgentName,
    bool IsSuccess,
    IReadOnlyList<string> Suggestions,
    double ConfidenceScore,
    long DurationMs
);
