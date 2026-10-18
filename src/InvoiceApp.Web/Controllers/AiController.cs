using InvoiceApp.Web.Models.ViewModels;
using InvoiceApp.Web.Services.Ai;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceApp.Web.Controllers;

/// <summary>
/// Controller managing AI Assistant web interactions, the dedicated AI Agent Studio view,
/// and antiforgery-protected chat proxy endpoints.
/// </summary>
[Route("ai")]
public sealed class AiController : Controller
{
    private readonly IAiAssistantService _aiAssistantService;
    private readonly ILogger<AiController> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="AiController"/>.
    /// </summary>
    /// <param name="aiAssistantService">The AI assistant orchestration service.</param>
    /// <param name="logger">Structured logger instance.</param>
    public AiController(IAiAssistantService aiAssistantService, ILogger<AiController> logger)
    {
        _aiAssistantService = aiAssistantService;
        _logger = logger;
    }

    /// <summary>
    /// Renders the full-screen interactive AI Agent Studio interface.
    /// </summary>
    /// <returns>The AI Studio view.</returns>
    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Handles asynchronous natural language chat interactions from the browser chat panels.
    /// </summary>
    /// <param name="request">The chat payload containing message, conversation ID, and page context.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A JSON response containing the agent reply, conversation ID, and suggestions.</returns>
    [HttpPost("chat")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Chat([FromBody] AiChatRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new { message = "Message content is required." });
        }

        try
        {
            var response = await _aiAssistantService.SendMessageAsync(request, cancellationToken);
            return Json(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing AI chat interaction");
            return StatusCode(500, new
            {
                message = "The AI Assistant encountered a temporary error. Please try again shortly.",
                conversationId = request.ConversationId ?? Guid.NewGuid().ToString("N")[..8],
                suggestions = new[] { "Show overdue invoices", "Summarize invoice activity" }
            });
        }
    }
}
