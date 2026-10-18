using InvoiceApp.AI.Common;
using InvoiceApp.AI.Telemetry;
using InvoiceApp.AI.Tools;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace InvoiceApp.AI.Agents;

/// <summary>
/// Specialized autonomous agent dedicated to platform onboarding, knowledge base retrieval (RAG),
/// and explaining business invariants and state machine rules.
/// </summary>
/// <remarks>
/// <para>
/// Integrates domain tools from <see cref="DocumentationTools"/> via Microsoft Agent Framework.
/// Employs Retrieval-Augmented Generation (RAG) principles by dynamically fetching and quoting
/// Markdown documentation files located in <c>Docs/</c>.
/// </para>
/// <para>
/// Handles user workflow inquiries such as:
/// </para>
/// <list type="bullet">
///   <item><description>"How do I create a new customer?"</description></item>
///   <item><description>"Can I delete an invoice that has already been sent to a client?"</description></item>
///   <item><description>"What are the allowed status transitions for an invoice?"</description></item>
/// </list>
/// </remarks>
public sealed class DocumentationAgent : BaseAgent
{
    private readonly DocumentationTools _docTools;

    /// <summary>
    /// Initializes a new instance of <see cref="DocumentationAgent"/>.
    /// </summary>
    /// <param name="chatClient">The underlying chat client instance.</param>
    /// <param name="docTools">The documentation domain tools provider.</param>
    /// <param name="telemetry">The telemetry service for execution tracking.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="options">The Azure OpenAI configuration options.</param>
    /// <param name="simulationMode">The simulation mode toggle.</param>
    public DocumentationAgent(
        IChatClient? chatClient,
        DocumentationTools docTools,
        ITelemetryService telemetry,
        ILogger<DocumentationAgent> logger,
        IOptions<AzureOpenAISettings> options,
        SimulationMode simulationMode)
        : base(
            chatClient,
            instructions:
            """
            You are the specialized Platform Documentation Agent for InvoiceAI.
            Your responsibilities include:
            1. Guiding users on how to use InvoiceAI features step-by-step.
            2. Answering questions about customer creation, invoice creation, and status management.
            3. Explaining accounting and audit rules (e.g. why only Drafts can be deleted, why customer emails must be unique).
            4. Reading official documentation pages using ReadDocumentationPage to ground your answers in factual system docs.
            """,
            name: "Platform Documentation Agent",
            description: "Guides users on how to navigate the platform, create customers and invoices, and understand business rules.",
            tools: docTools.GetTools(),
            telemetry: telemetry,
            logger: logger,
            simulationMode: simulationMode)
    {
        _docTools = docTools;
    }

    /// <inheritdoc />
    protected override async Task<string> GenerateSimulatedResponseAsync(AgentRequest request, CancellationToken cancellationToken)
    {
        var lower = request.Prompt.ToLowerInvariant();

        if (lower.Contains("customer") || lower.Contains("create customer"))
        {
            var doc = await _docTools.ReadDocumentationPage("creating-customer", cancellationToken);
            return $"📖 [SIMULATION]: Here is the official guide on creating customers in InvoiceAI:\n\n{doc}";
        }

        if (lower.Contains("invoice") || lower.Contains("create invoice"))
        {
            var doc = await _docTools.ReadDocumentationPage("creating-invoice", cancellationToken);
            return $"📖 [SIMULATION]: Here is the official guide on creating invoices in InvoiceAI:\n\n{doc}";
        }

        if (lower.Contains("status") || lower.Contains("delete") || lower.Contains("manage"))
        {
            var doc = await _docTools.ReadDocumentationPage("managing-invoices", cancellationToken);
            return $"📖 [SIMULATION]: Here is the guide on managing invoices and lifecycle state transitions:\n\n{doc}";
        }

        var general = await _docTools.ReadDocumentationPage("getting-started", cancellationToken);
        return $"📖 [SIMULATION]: Welcome to InvoiceAI! Here is an overview from the platform docs:\n\n{general}";
    }
}
