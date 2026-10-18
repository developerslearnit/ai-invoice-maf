using InvoiceApp.Web.Models.Api;
using InvoiceApp.Web.Models.ViewModels;

namespace InvoiceApp.Web.Services.ApiClients;

/// <summary>
/// Typed HTTP client implementation for communicating with the AI Assistant endpoint on the backend API.
/// </summary>
public sealed class AiApiClient : BaseApiClient, IAiApiClient
{
    /// <summary>
    /// Initializes a new instance of <see cref="AiApiClient"/>.
    /// </summary>
    /// <param name="httpClient">Configured HTTP client instance with base address pointing to InvoiceApp.Api.</param>
    /// <param name="logger">Structured logger instance.</param>
    public AiApiClient(HttpClient httpClient, ILogger<AiApiClient> logger)
        : base(httpClient, logger)
    {
    }

    /// <inheritdoc />
    public async Task<ApiResponse<AiChatApiResponseModel>> SendChatMessageAsync(AiChatRequest request, CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            message = request.Message,
            conversationId = request.ConversationId,
            context = request.Context is not null
                ? new Dictionary<string, object>
                {
                    ["page"] = request.Context.Page ?? string.Empty,
                    ["invoiceId"] = request.Context.InvoiceId ?? string.Empty,
                    ["invoiceNumber"] = request.Context.InvoiceNumber ?? string.Empty,
                    ["customerId"] = request.Context.CustomerId ?? string.Empty,
                    ["customerName"] = request.Context.CustomerName ?? string.Empty
                }
                : null
        };

        return await PostAsJsonAsync<object, AiChatApiResponseModel>("api/ai/chat", payload, cancellationToken);
    }
}
