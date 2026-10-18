using InvoiceApp.Web.Models.ViewModels;
using InvoiceApp.Web.Services.ApiClients;

namespace InvoiceApp.Web.Services.Ai;

/// <summary>
/// Production implementation of <see cref="IAiAssistantService"/> that forwards prompts to
/// Microsoft Agent Framework via the backend API, with automatic fallback to local simulation.
/// </summary>
/// <remarks>
/// <para>
/// 1. Dispatches the user request to the backend <see cref="IAiApiClient"/> (<c>POST /api/ai/chat</c>).
/// </para>
/// <para>
/// 2. If the backend API responds successfully, returns the agent's live response and telemetry metadata.
/// </para>
/// <para>
/// 3. If the backend is unreachable or returns a failure status, gracefully falls back to <see cref="MockAiAssistantService"/>
/// to ensure the frontend user experience never breaks during development or offline demos.
/// </para>
/// </remarks>
public sealed class AgentFrameworkAiAssistantService : IAiAssistantService
{
    private readonly IAiApiClient _aiApiClient;
    private readonly MockAiAssistantService _mockFallbackService;
    private readonly ILogger<AgentFrameworkAiAssistantService> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="AgentFrameworkAiAssistantService"/>.
    /// </summary>
    /// <param name="aiApiClient">The typed HTTP client for talking to the AI API.</param>
    /// <param name="mockFallbackService">The local simulation fallback service.</param>
    /// <param name="logger">Structured logger instance.</param>
    public AgentFrameworkAiAssistantService(
        IAiApiClient aiApiClient,
        MockAiAssistantService mockFallbackService,
        ILogger<AgentFrameworkAiAssistantService> logger)
    {
        _aiApiClient = aiApiClient;
        _mockFallbackService = mockFallbackService;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<AiChatResponse> SendMessageAsync(AiChatRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var apiResult = await _aiApiClient.SendChatMessageAsync(request, cancellationToken);
            if (apiResult.IsSuccess && apiResult.Data is not null)
            {
                var data = apiResult.Data;
                return new AiChatResponse(
                    Message: data.Message,
                    ConversationId: data.ConversationId,
                    Suggestions: data.Suggestions,
                    AgentName: data.AgentName,
                    DurationMs: data.DurationMs,
                    ConfidenceScore: data.ConfidenceScore,
                    IsSuccess: data.IsSuccess
                );
            }

            _logger.LogWarning("AI API returned unsuccessful result ({Status}: {Error}). Falling back to local simulation service.",
                apiResult.StatusCode, apiResult.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not reach Microsoft Agent Framework API. Falling back to local simulation service.");
        }

        return await _mockFallbackService.SendMessageAsync(request, cancellationToken);
    }
}
