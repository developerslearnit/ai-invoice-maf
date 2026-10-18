using System.Text;
using InvoiceApp.Web.Models.ViewModels;
using InvoiceApp.Web.Services.ApiClients;

namespace InvoiceApp.Web.Services.Ai;

/// <summary>
/// Simulation and fallback implementation of <see cref="IAiAssistantService"/> for local offline testing.
/// </summary>
/// <remarks>
/// Directly queries typed API clients to provide realistic answers for invoice summaries, overdue debts,
/// and customer balances when live AI models or backend AI endpoints are unavailable.
/// </remarks>
public sealed class MockAiAssistantService : IAiAssistantService
{
    private readonly IInvoiceApiClient _invoiceApiClient;
    private readonly ICustomerApiClient _customerApiClient;
    private readonly ILogger<MockAiAssistantService> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="MockAiAssistantService"/>.
    /// </summary>
    /// <param name="invoiceApiClient">Typed API client for invoices.</param>
    /// <param name="customerApiClient">Typed API client for customers.</param>
    /// <param name="logger">Structured logger instance.</param>
    public MockAiAssistantService(
        IInvoiceApiClient invoiceApiClient,
        ICustomerApiClient customerApiClient,
        ILogger<MockAiAssistantService> logger)
    {
        _invoiceApiClient = invoiceApiClient;
        _customerApiClient = customerApiClient;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<AiChatResponse> SendMessageAsync(AiChatRequest request, CancellationToken cancellationToken = default)
    {
        var conversationId = string.IsNullOrWhiteSpace(request.ConversationId)
            ? Guid.NewGuid().ToString("N")[..8]
            : request.ConversationId;

        var userMessage = request.Message.Trim().ToLowerInvariant();
        _logger.LogInformation("Processing mock AI query: '{Message}' (Context: {@Context})", request.Message, request.Context);

        // 1. Contextual query (e.g. user is on an Invoice Details page and says "summarize this invoice")
        if (request.Context?.Page == "InvoiceDetails" && !string.IsNullOrEmpty(request.Context.InvoiceId))
        {
            if (Guid.TryParse(request.Context.InvoiceId, out var invoiceId))
            {
                var invoiceResult = await _invoiceApiClient.GetInvoiceByIdAsync(invoiceId, cancellationToken);
                if (invoiceResult.IsSuccess && invoiceResult.Data is not null)
                {
                    var inv = invoiceResult.Data;
                    var summaryText = new StringBuilder();
                    summaryText.AppendLine($"📄 **Invoice Summary for {inv.InvoiceNumber}**");
                    summaryText.AppendLine($"- **Customer:** {inv.Customer.Name} ({inv.Customer.Email})");
                    summaryText.AppendLine($"- **Status:** {inv.Status}");
                    summaryText.AppendLine($"- **Issue Date:** {DateHelper.Format(inv.IssueDate)} | **Due Date:** {DateHelper.Format(inv.DueDate)}");
                    summaryText.AppendLine($"- **Total Amount:** {CurrencyHelper.FormatNaira(inv.TotalAmount)} across {inv.Items.Count} line items.");
                    if (!string.IsNullOrWhiteSpace(inv.Notes))
                    {
                        summaryText.AppendLine($"- **Notes:** \"{inv.Notes}\"");
                    }
                    summaryText.AppendLine();
                    summaryText.AppendLine("💡 *Context detected from active page. When Microsoft Agent Framework is integrated, agents will leverage this context automatically via semantic tool bindings.*");

                    return new AiChatResponse(
                        summaryText.ToString(),
                        conversationId,
                        ["Show overdue invoices", "Summarize overall business", "Return to customer details"]
                    );
                }
            }
        }

        // 2. Overdue invoices query
        if (userMessage.Contains("overdue") || userMessage.Contains("unpaid") || userMessage.Contains("late"))
        {
            var overdueResult = await _invoiceApiClient.GetOverdueInvoicesAsync(cancellationToken);
            if (overdueResult.IsSuccess && overdueResult.Data is not null)
            {
                var overdueList = overdueResult.Data;
                if (overdueList.Count == 0)
                {
                    return new AiChatResponse(
                        "🎉 **Great news!** You currently have **0 overdue invoices** across all customers. All accounts are up-to-date.",
                        conversationId,
                        ["Summarize invoice activity", "Which customer owes the most?", "Show all invoices"]
                    );
                }

                var sb = new StringBuilder();
                var totalOverdueAmount = overdueList.Sum(i => i.TotalAmount);
                sb.AppendLine($"⚠️ You currently have **{overdueList.Count} overdue invoice(s)** totaling **{CurrencyHelper.FormatNaira(totalOverdueAmount)}**:");
                sb.AppendLine();
                foreach (var inv in overdueList.Take(5))
                {
                    sb.AppendLine($"• **{inv.InvoiceNumber}** — {inv.CustomerName}: {CurrencyHelper.FormatNaira(inv.TotalAmount)} (Due: {DateHelper.Format(inv.DueDate)})");
                }

                if (overdueList.Count > 5)
                {
                    sb.AppendLine($"• *...and {overdueList.Count - 5} more overdue invoices.*");
                }

                sb.AppendLine();
                sb.AppendLine("Would you like me to highlight the customers with the largest outstanding balance?");

                return new AiChatResponse(
                    sb.ToString(),
                    conversationId,
                    ["Which customer owes the most?", "Summarize invoice activity", "Show sent invoices"]
                );
            }
        }

        // 3. Overall business summary / invoice activity
        if (userMessage.Contains("summary") || userMessage.Contains("activity") || userMessage.Contains("status") || userMessage.Contains("overview"))
        {
            var summaryResult = await _invoiceApiClient.GetInvoiceSummaryAsync(cancellationToken);
            if (summaryResult.IsSuccess && summaryResult.Data is not null)
            {
                var s = summaryResult.Data;
                var sb = new StringBuilder();
                sb.AppendLine("📊 **Current Billing & Invoicing Snapshot**");
                sb.AppendLine($"- **Total Invoices Created:** {s.TotalInvoices}");
                sb.AppendLine($"- **Drafts:** {s.DraftInvoices} | **Sent (Pending Payment):** {s.SentInvoices}");
                sb.AppendLine($"- **Paid Invoices:** {s.PaidInvoices} ({CurrencyHelper.FormatNaira(s.TotalPaidAmount)})");
                sb.AppendLine($"- **Overdue Invoices:** {s.OverdueInvoices} ({CurrencyHelper.FormatNaira(s.TotalOutstandingAmount)} outstanding)");
                sb.AppendLine($"- **Total Value Invoiced:** {CurrencyHelper.FormatNaira(s.TotalInvoiceAmount)}");
                sb.AppendLine();
                sb.AppendLine("✨ *In the next phase, Microsoft Agent Framework will allow you to generate custom reports and trigger email reminders via AI tools!*");

                return new AiChatResponse(
                    sb.ToString(),
                    conversationId,
                    ["Show overdue invoices", "Which customer owes the most?", "Show draft invoices"]
                );
            }
        }

        // 4. Customer balance queries ("who owes", "customer owes", "Contoso", "Northwind", etc.)
        if (userMessage.Contains("customer") || userMessage.Contains("owes") || userMessage.Contains("contoso") || userMessage.Contains("northwind"))
        {
            var customersResult = await _customerApiClient.GetCustomersAsync(cancellationToken);
            if (customersResult.IsSuccess && customersResult.Data is not null && customersResult.Data.Count > 0)
            {
                var customer = customersResult.Data.FirstOrDefault(c => userMessage.Contains(c.Name.ToLowerInvariant()))
                               ?? customersResult.Data.First();

                var customerSummary = await _customerApiClient.GetCustomerSummaryAsync(customer.Id, cancellationToken);
                if (customerSummary.IsSuccess && customerSummary.Data is not null)
                {
                    var cs = customerSummary.Data;
                    var sb = new StringBuilder();
                    sb.AppendLine($"🏢 **Customer Breakdown: {cs.CustomerName}**");
                    sb.AppendLine($"- **Total Invoices:** {cs.TotalInvoices} ({CurrencyHelper.FormatNaira(cs.TotalInvoiced)})");
                    sb.AppendLine($"- **Paid:** {cs.PaidInvoices} invoices ({CurrencyHelper.FormatNaira(cs.TotalPaid)})");
                    sb.AppendLine($"- **Outstanding:** {cs.OutstandingInvoices} invoices ({CurrencyHelper.FormatNaira(cs.TotalOutstanding)})");

                    return new AiChatResponse(
                        sb.ToString(),
                        conversationId,
                        ["Show overdue invoices", "Summarize invoice activity", "Show all customers"]
                    );
                }
            }
        }

        // 5. Default / fallback guidance
        var defaultResponse = new StringBuilder();
        defaultResponse.AppendLine($"Hello! I received your query: *\"{request.Message}\"*.");
        defaultResponse.AppendLine();
        defaultResponse.AppendLine("I am currently operating using **MockAiAssistantService** to simulate upcoming AI capabilities for training purposes.");
        defaultResponse.AppendLine();
        defaultResponse.AppendLine("You can try testing these sample workflows right now:");
        defaultResponse.AppendLine("1. Ask **\"Show overdue invoices\"** to view live late billing items.");
        defaultResponse.AppendLine("2. Ask **\"Summarize invoice activity\"** for executive KPIs.");
        defaultResponse.AppendLine("3. Ask **\"Which customer owes the most?\"** for debtor inquiries.");
        defaultResponse.AppendLine();
        defaultResponse.AppendLine("🚀 In the next stage, this service will be replaced with **Microsoft Agent Framework** to orchestrate LLM reasoning and real-time backend tool calling.");

        return new AiChatResponse(
            defaultResponse.ToString(),
            conversationId,
            ["Show overdue invoices", "Summarize invoice activity", "Which customer owes the most?"]
        );
    }
}
