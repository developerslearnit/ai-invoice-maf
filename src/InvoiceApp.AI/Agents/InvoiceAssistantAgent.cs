using System.Text.RegularExpressions;
using InvoiceApp.AI.Common;
using InvoiceApp.AI.Telemetry;
using InvoiceApp.AI.Tools;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace InvoiceApp.AI.Agents;

/// <summary>
/// Specialized autonomous agent responsible for invoice lifecycle management, line-item billing calculations,
/// state machine transitions, and draft deletions.
/// </summary>
/// <remarks>
/// <para>
/// Binds domain tools from <see cref="InvoiceTools"/> and <see cref="CustomerTools"/> via Microsoft Agent Framework.
/// Enforces business domain rules:
/// </para>
/// <list type="bullet">
///   <item><description>Sequential <c>INV-00000X</c> numbering generation.</description></item>
///   <item><description>State machine status transitions (Draft $\rightarrow$ Sent $\rightarrow$ Paid / Overdue / Cancelled).</description></item>
///   <item><description>Audit invariant: only <c>Draft</c> invoices can be deleted; non-drafts must be <c>Cancelled</c>.</description></item>
///   <item><description>Currency formatting in Nigerian Naira (₦).</description></item>
/// </list>
/// </remarks>
public sealed class InvoiceAssistantAgent : BaseAgent
{
    private readonly InvoiceTools _invoiceTools;

    /// <summary>
    /// Initializes a new instance of <see cref="InvoiceAssistantAgent"/>.
    /// </summary>
    /// <param name="chatClient">The chat client instance (null in simulation mode).</param>
    /// <param name="invoiceTools">The invoice domain tools provider.</param>
    /// <param name="customerTools">The customer domain tools provider for customer resolution.</param>
    /// <param name="telemetry">The telemetry service.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="options">The Azure OpenAI configuration options.</param>
    /// <param name="simulationMode">The simulation mode toggle.</param>
    public InvoiceAssistantAgent(
        IChatClient? chatClient,
        InvoiceTools invoiceTools,
        CustomerTools customerTools,
        ITelemetryService telemetry,
        ILogger<InvoiceAssistantAgent> logger,
        IOptions<AzureOpenAISettings> options,
        SimulationMode simulationMode)
        : base(
            chatClient,
            instructions:
            """
            You are the specialized Invoice Operations Agent for InvoiceAI.
            Your responsibilities include:
            1. Creating new itemized invoices for customers using the CreateInvoice tool.
            2. Looking up detailed invoice records by invoice number (e.g., INV-000001) using GetInvoiceByNumber.
            3. Updating invoice statuses (Draft, Sent, Paid, Overdue, Cancelled) via UpdateInvoiceStatus according to the state machine.
            4. Deleting draft invoices via DeleteDraftInvoice (remember: only Drafts can be deleted; non-drafts must be Cancelled).
            5. Always confirm the invoice number, total amount (in Nigerian Naira ₦), customer name, and line items when performing actions.
            6. Present currency values clearly in Naira (₦) with commas and two decimals.
            """,
            name: "Invoice Operations Agent",
            description: "Creates invoices, queries details by number, updates statuses (Paid, Sent, Overdue, Cancelled), and manages draft deletions.",
            tools: invoiceTools.GetTools().Concat(customerTools.GetTools()),
            telemetry: telemetry,
            logger: logger,
            simulationMode: simulationMode)
    {
        _invoiceTools = invoiceTools;
    }

    /// <inheritdoc />
    protected override async Task<string> GenerateSimulatedResponseAsync(AgentRequest request, CancellationToken cancellationToken)
    {
        var lower = request.Prompt.ToLowerInvariant();

        // 1. Status change simulation
        if (lower.Contains("mark") || lower.Contains("paid") || lower.Contains("status") || lower.Contains("cancel"))
        {
            if (lower.Contains("inv-"))
            {
                var match = Regex.Match(request.Prompt, @"INV-\d+", RegexOptions.IgnoreCase);
                var invNum = match.Success ? match.Value.ToUpperInvariant() : "INV-000001";
                var targetStatus = lower.Contains("paid") ? "Paid" : lower.Contains("sent") ? "Sent" : lower.Contains("cancel") ? "Cancelled" : "Overdue";

                var toolResult = await _invoiceTools.UpdateInvoiceStatus(invNum, targetStatus, cancellationToken);
                return $"✅ [SIMULATION]: {toolResult}\n\nInvoice **{invNum}** has been transitioned to **{targetStatus}**. Customer balance and dashboard revenue metrics have been updated.";
            }
        }

        // 2. Overdue inquiry
        if (lower.Contains("overdue") || lower.Contains("late"))
        {
            var overdue = await _invoiceTools.GetOverdueInvoices(cancellationToken);
            return $"⚠️ [SIMULATION]: Live overdue invoice analysis executed:\n\n{overdue}";
        }

        // 3. Create invoice inquiry
        if (lower.Contains("create invoice") || lower.Contains("new invoice"))
        {
            return "📝 [SIMULATION]: To create an invoice, I can call the `CreateInvoice` tool with customer details, due date, and dynamic line items. For example: *'Create invoice for Apex Corp with 10 hours of design at ₦25,000, due 2026-11-01'*.";
        }

        return await base.GenerateSimulatedResponseAsync(request, cancellationToken);
    }
}
