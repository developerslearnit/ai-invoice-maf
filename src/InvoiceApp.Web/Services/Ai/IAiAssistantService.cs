using InvoiceApp.Web.Models.ViewModels;

namespace InvoiceApp.Web.Services.Ai;

/// <summary>
/// Service contract for handling AI Assistant interactions from MVC controllers and views.
/// </summary>
public interface IAiAssistantService
{
    /// <summary>
    /// Processes a user chat request by communicating with Microsoft Agent Framework via the backend API,
    /// or seamlessly falling back to local simulation if the backend is unreachable.
    /// </summary>
    /// <param name="request">The chat request containing message, conversation ID, and page context.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A structured <see cref="AiChatResponse"/> with message text, agent attribution, and suggestions.</returns>
    Task<AiChatResponse> SendMessageAsync(AiChatRequest request, CancellationToken cancellationToken = default);
}
