using InvoiceApp.AI.Common;
using InvoiceApp.AI.Telemetry;
using InvoiceApp.AI.Tools;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace InvoiceApp.AI.Agents;

/// <summary>
/// Specialized autonomous agent focusing on billing metrics, executive KPIs, revenue tracking,
/// and debtor aging exposure.
/// </summary>
/// <remarks>
/// <para>
/// Integrates domain tools from <see cref="InvoiceTools"/> and <see cref="CustomerTools"/> via Microsoft Agent Framework.
/// Answers executive analytical queries such as:
/// </para>
/// <list type="bullet">
///   <item><description>"What is our total outstanding balance?"</description></item>
///   <item><description>"Which customer owes the most money?"</description></item>
///   <item><description>"Summarize invoice activity and collection efficiency."</description></item>
/// </list>
/// </remarks>
public sealed class AnalyticsAgent : BaseAgent
{
    private readonly InvoiceTools _invoiceTools;

    /// <summary>
    /// Initializes a new instance of <see cref="AnalyticsAgent"/>.
    /// </summary>
    /// <param name="chatClient">The underlying chat client instance.</param>
    /// <param name="invoiceTools">The invoice domain tools provider.</param>
    /// <param name="customerTools">The customer domain tools provider.</param>
    /// <param name="telemetry">The telemetry service for execution tracking.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="options">The Azure OpenAI configuration options.</param>
    /// <param name="simulationMode">The simulation mode toggle.</param>
    public AnalyticsAgent(
        IChatClient? chatClient,
        InvoiceTools invoiceTools,
        CustomerTools customerTools,
        ITelemetryService telemetry,
        ILogger<AnalyticsAgent> logger,
        IOptions<AzureOpenAISettings> options,
        SimulationMode simulationMode)
        : base(
            chatClient,
            instructions:
            """
            You are the specialized Billing Analytics Agent for InvoiceAI.
            Your responsibilities include:
            1. Analyzing executive KPIs (Total Invoiced, Total Collected, Total Outstanding, and Overdue balances) using GetInvoiceSummary.
            2. Identifying top overdue debtors and evaluating collections risk using GetOverdueInvoices.
            3. Evaluating customer lifetime value and concentration risk using GetCustomerProfile.
            4. Explaining status distribution trends across Draft, Sent, Paid, Overdue, and Cancelled invoices.
            5. Always format currency numbers cleanly in Nigerian Naira (₦).
            """,
            name: "Billing Analytics Agent",
            description: "Analyzes financial KPIs, revenue collected, overdue debtor balances, and pipeline status distribution.",
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
        var summary = await _invoiceTools.GetInvoiceSummary(cancellationToken);
        return $"📊 [SIMULATION]: Live executive billing metrics retrieved:\n\n{summary}";
    }
}
