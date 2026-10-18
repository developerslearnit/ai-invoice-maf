using System.ComponentModel;
using System.Text.Json;
using InvoiceApp.Application.Common.Messaging;
using InvoiceApp.Application.DTOs;
using InvoiceApp.Application.Features.Customers.Queries.GetCustomers;
using InvoiceApp.Application.Features.Invoices.Commands.CreateInvoice;
using InvoiceApp.Application.Features.Invoices.Commands.DeleteInvoice;
using InvoiceApp.Application.Features.Invoices.Commands.UpdateInvoiceStatus;
using InvoiceApp.Application.Features.Invoices.Queries.GetInvoiceById;
using InvoiceApp.Application.Features.Invoices.Queries.GetInvoiceByNumber;
using InvoiceApp.Application.Features.Invoices.Queries.GetInvoices;
using InvoiceApp.Application.Features.Invoices.Queries.GetInvoiceSummary;
using InvoiceApp.Application.Features.Invoices.Queries.GetOverdueInvoices;
using InvoiceApp.Domain.Enums;
using InvoiceApp.Domain.Models;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace InvoiceApp.AI.Tools;

/// <summary>
/// Exposes invoice lifecycle and financial operations as autonomous AI tools (<see cref="AITool"/>)
/// callable by agents using Microsoft Agent Framework and Microsoft.Extensions.AI.
/// </summary>
/// <remarks>
/// <para>
/// Binds Clean Architecture application commands and queries through <see cref="IDispatcher"/>.
/// Handles customer name-to-ID resolution, line item JSON deserialization, invoice numbering,
/// state machine transitions, and draft invoice deletions.
/// </para>
/// <para>
/// Every tool method is decorated with <see cref="DescriptionAttribute"/> on both the method and
/// its parameters to generate rich JSON Schema contracts for LLM function calling.
/// </para>
/// </remarks>
/// <param name="dispatcher">The in-process mediator for dispatching CQRS commands and queries.</param>
/// <param name="logger">Structured logger instance.</param>
public sealed class InvoiceTools(
    IDispatcher dispatcher,
    ILogger<InvoiceTools> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true, PropertyNameCaseInsensitive = true };

    /// <summary>
    /// Generates the list of <see cref="AITool"/> descriptors registered with the LLM via <see cref="AIFunctionFactory.Create"/>.
    /// </summary>
    /// <returns>An enumerable of AI function definitions for invoice operations.</returns>
    public IEnumerable<AITool> GetTools()
    {
        yield return AIFunctionFactory.Create(
            CreateInvoice,
            nameof(CreateInvoice),
            "Creates a new draft invoice with line items for a specified customer. Generates sequential invoice number.");

        yield return AIFunctionFactory.Create(
            GetInvoiceByNumber,
            nameof(GetInvoiceByNumber),
            "Retrieves full invoice details by sequential invoice number (e.g. 'INV-000001').");

        yield return AIFunctionFactory.Create(
            UpdateInvoiceStatus,
            nameof(UpdateInvoiceStatus),
            "Transitions an invoice status to Sent, Paid, Overdue, or Cancelled according to state machine rules.");

        yield return AIFunctionFactory.Create(
            DeleteDraftInvoice,
            nameof(DeleteDraftInvoice),
            "Permanently deletes an invoice. ONLY permitted if invoice is in Draft status.");

        yield return AIFunctionFactory.Create(
            GetOverdueInvoices,
            nameof(GetOverdueInvoices),
            "Retrieves all overdue invoices across all customers with total overdue balance.");

        yield return AIFunctionFactory.Create(
            GetInvoiceSummary,
            nameof(GetInvoiceSummary),
            "Retrieves global billing summary, total invoice counts by status, revenue collected, and total outstanding debt.");

        yield return AIFunctionFactory.Create(
            ListInvoices,
            nameof(ListInvoices),
            "Lists invoices with optional status filter.");
    }

    /// <summary>
    /// Creates a new draft invoice with itemized line items for a specified customer.
    /// </summary>
    /// <param name="customerIdentifier">The GUID string or business name of the customer.</param>
    /// <param name="dueDate">The payment due date in YYYY-MM-DD format (must be on or after the issue date).</param>
    /// <param name="itemsJson">JSON array of itemized line items (e.g., '[{"description": "Consulting", "quantity": 10, "unitPrice": 25000}]').</param>
    /// <param name="notes">Optional payment terms or delivery notes.</param>
    /// <param name="issueDate">Optional issue date in YYYY-MM-DD format (defaults to current date).</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A JSON string detailing the created invoice number, total amount, and status.</returns>
    [Description("Creates a new invoice for a customer with itemized line items.")]
    public async Task<string> CreateInvoice(
        [Description("Customer GUID or exact customer name")] string customerIdentifier,
        [Description("Payment due date in YYYY-MM-DD format (must be on or after issue date)")] string dueDate,
        [Description("JSON array of line items: [{\"description\": \"Consulting\", \"quantity\": 10, \"unitPrice\": 25000}]")] string itemsJson,
        [Description("Optional invoice notes or payment terms")] string? notes = null,
        [Description("Optional issue date in YYYY-MM-DD format (defaults to today)")] string? issueDate = null,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Tool invoked: CreateInvoice for customer '{Customer}'", customerIdentifier);

        // 1. Resolve Customer ID
        Guid customerId;
        if (Guid.TryParse(customerIdentifier, out var parsedGuid))
        {
            customerId = parsedGuid;
        }
        else
        {
            var customersResult = await dispatcher.QueryAsync(new GetCustomersQuery(), cancellationToken);
            var match = customersResult.Value?.FirstOrDefault(c =>
                c.Name.Equals(customerIdentifier.Trim(), StringComparison.OrdinalIgnoreCase) ||
                c.Email.Equals(customerIdentifier.Trim(), StringComparison.OrdinalIgnoreCase));

            if (match is null)
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    error = $"Could not find customer matching '{customerIdentifier}'. Please verify customer name or create customer first."
                }, JsonOptions);
            }

            customerId = match.Id;
        }

        // 2. Parse Dates
        var parsedIssueDate = !string.IsNullOrWhiteSpace(issueDate) && DateOnly.TryParse(issueDate, out var id)
            ? id
            : DateOnly.FromDateTime(DateTime.UtcNow);

        var parsedDueDate = !string.IsNullOrWhiteSpace(dueDate) && DateOnly.TryParse(dueDate, out var dd)
            ? dd
            : parsedIssueDate.AddDays(14);

        // 3. Parse Line Items
        List<CreateInvoiceItemRequest> items;
        try
        {
            items = JsonSerializer.Deserialize<List<CreateInvoiceItemRequest>>(itemsJson, JsonOptions) ?? [];
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = $"Failed to parse line items JSON. Expected format: [{{\"description\": \"...\", \"quantity\": 1, \"unitPrice\": 100}}]. Details: {ex.Message}"
            }, JsonOptions);
        }

        if (items.Count == 0)
        {
            return JsonSerializer.Serialize(new { success = false, error = "At least one line item is required." }, JsonOptions);
        }

        var command = new CreateInvoiceCommand(customerId, parsedIssueDate, parsedDueDate, notes, items);
        var result = await dispatcher.SendAsync(command, cancellationToken);

        if (result.IsFailure)
        {
            return JsonSerializer.Serialize(new { success = false, error = result.Error.Description }, JsonOptions);
        }

        // Fetch newly created invoice to return number
        var invoiceResult = await dispatcher.QueryAsync(new GetInvoiceByIdQuery(result.Value), cancellationToken);
        var createdInvoice = invoiceResult.Value;

        return JsonSerializer.Serialize(new
        {
            success = true,
            invoiceId = result.Value,
            invoiceNumber = createdInvoice?.InvoiceNumber ?? "Generated",
            totalAmount = createdInvoice?.TotalAmount ?? 0,
            status = "Draft",
            message = $"Invoice {createdInvoice?.InvoiceNumber} created successfully in Draft status for total amount ₦{createdInvoice?.TotalAmount:N2}."
        }, JsonOptions);
    }

    /// <summary>
    /// Retrieves complete invoice details including customer information, item lines, and status by sequential invoice number.
    /// </summary>
    /// <param name="invoiceNumber">The formatted invoice number (e.g. 'INV-000001').</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A JSON string containing the invoice record.</returns>
    [Description("Gets invoice details by sequential invoice number (e.g., 'INV-000001').")]
    public async Task<string> GetInvoiceByNumber(
        [Description("The invoice number to find (e.g. 'INV-000001')")] string invoiceNumber,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Tool invoked: GetInvoiceByNumber for '{InvoiceNumber}'", invoiceNumber);

        var result = await dispatcher.QueryAsync(new GetInvoiceByNumberQuery(invoiceNumber.Trim()), cancellationToken);
        if (result.IsFailure || result.Value is null)
        {
            return JsonSerializer.Serialize(new { success = false, error = result.Error.Description }, JsonOptions);
        }

        return JsonSerializer.Serialize(result.Value, JsonOptions);
    }

    /// <summary>
    /// Updates an invoice's lifecycle status in accordance with domain state machine transition invariants.
    /// </summary>
    /// <param name="invoiceIdentifier">The invoice number (e.g. 'INV-000001') or GUID identifier.</param>
    /// <param name="newStatus">The desired target status: 'Draft', 'Sent', 'Paid', 'Overdue', or 'Cancelled'.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A JSON string indicating success or state transition violation error.</returns>
    [Description("Updates the lifecycle status of an invoice (Draft, Sent, Paid, Overdue, Cancelled).")]
    public async Task<string> UpdateInvoiceStatus(
        [Description("The invoice number (e.g. 'INV-000001') or invoice GUID")] string invoiceIdentifier,
        [Description("Target status: 'Draft', 'Sent', 'Paid', 'Overdue', or 'Cancelled'")] string newStatus,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Tool invoked: UpdateInvoiceStatus for '{Invoice}' to '{Status}'", invoiceIdentifier, newStatus);

        if (!Enum.TryParse<InvoiceStatus>(newStatus.Trim(), ignoreCase: true, out var targetStatus))
        {
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = $"Invalid invoice status '{newStatus}'. Allowed: Draft, Sent, Paid, Overdue, Cancelled."
            }, JsonOptions);
        }

        // Resolve invoice ID
        Guid invoiceId;
        if (Guid.TryParse(invoiceIdentifier, out var parsedGuid))
        {
            invoiceId = parsedGuid;
        }
        else
        {
            var byNumber = await dispatcher.QueryAsync(new GetInvoiceByNumberQuery(invoiceIdentifier.Trim()), cancellationToken);
            if (byNumber.IsFailure || byNumber.Value is null)
            {
                return JsonSerializer.Serialize(new { success = false, error = $"Invoice '{invoiceIdentifier}' not found." }, JsonOptions);
            }
            invoiceId = byNumber.Value.Id;
        }

        var command = new UpdateInvoiceStatusCommand(invoiceId, targetStatus);
        var result = await dispatcher.SendAsync(command, cancellationToken);

        if (result.IsFailure)
        {
            return JsonSerializer.Serialize(new { success = false, error = result.Error.Description }, JsonOptions);
        }

        return JsonSerializer.Serialize(new
        {
            success = true,
            invoiceId,
            newStatus = targetStatus.ToString(),
            message = $"Invoice '{invoiceIdentifier}' status successfully changed to {targetStatus}."
        }, JsonOptions);
    }

    /// <summary>
    /// Deletes an invoice permanently from the database. Enforces the strict rule that only Draft invoices can be deleted.
    /// </summary>
    /// <param name="invoiceIdentifier">The invoice number (e.g. 'INV-000001') or GUID identifier.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A JSON string indicating successful deletion or error if invoice was already sent or paid.</returns>
    [Description("Deletes an invoice. Only invoices in Draft status can be deleted.")]
    public async Task<string> DeleteDraftInvoice(
        [Description("The invoice number (e.g. 'INV-000001') or GUID")] string invoiceIdentifier,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Tool invoked: DeleteDraftInvoice for '{Invoice}'", invoiceIdentifier);

        Guid invoiceId;
        if (Guid.TryParse(invoiceIdentifier, out var parsedGuid))
        {
            invoiceId = parsedGuid;
        }
        else
        {
            var byNumber = await dispatcher.QueryAsync(new GetInvoiceByNumberQuery(invoiceIdentifier.Trim()), cancellationToken);
            if (byNumber.IsFailure || byNumber.Value is null)
            {
                return JsonSerializer.Serialize(new { success = false, error = $"Invoice '{invoiceIdentifier}' not found." }, JsonOptions);
            }
            invoiceId = byNumber.Value.Id;
        }

        var command = new DeleteInvoiceCommand(invoiceId);
        var result = await dispatcher.SendAsync(command, cancellationToken);

        if (result.IsFailure)
        {
            return JsonSerializer.Serialize(new { success = false, error = result.Error.Description }, JsonOptions);
        }

        return JsonSerializer.Serialize(new
        {
            success = true,
            invoiceId,
            message = $"Draft invoice '{invoiceIdentifier}' was permanently deleted."
        }, JsonOptions);
    }

    /// <summary>
    /// Retrieves all invoices currently past their payment due date across all customers, along with total overdue exposure.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A JSON string containing the count, total overdue amount, and list of overdue invoices.</returns>
    [Description("Retrieves all overdue invoices across the entire platform.")]
    public async Task<string> GetOverdueInvoices(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Tool invoked: GetOverdueInvoices");

        var result = await dispatcher.QueryAsync(new GetOverdueInvoicesQuery(), cancellationToken);
        if (result.IsFailure || result.Value is null)
        {
            return JsonSerializer.Serialize(new { success = false, error = "Unable to retrieve overdue invoices." }, JsonOptions);
        }

        var list = result.Value;
        return JsonSerializer.Serialize(new
        {
            success = true,
            count = list.Count,
            totalOverdueAmount = list.Sum(i => i.TotalAmount),
            invoices = list
        }, JsonOptions);
    }

    /// <summary>
    /// Retrieves executive billing metrics and status aggregations (Total, Draft, Sent, Paid, Overdue, and gross revenue).
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A JSON string containing the overall invoice summary.</returns>
    [Description("Retrieves overall invoice metrics and KPIs across all statuses.")]
    public async Task<string> GetInvoiceSummary(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Tool invoked: GetInvoiceSummary");

        var result = await dispatcher.QueryAsync(new GetInvoiceSummaryQuery(), cancellationToken);
        if (result.IsFailure || result.Value is null)
        {
            return JsonSerializer.Serialize(new { success = false, error = "Unable to retrieve invoice summary." }, JsonOptions);
        }

        return JsonSerializer.Serialize(result.Value, JsonOptions);
    }

    /// <summary>
    /// Retrieves a paginated list of invoices with optional lifecycle status filtering.
    /// </summary>
    /// <param name="status">Optional status filter (Draft, Sent, Paid, Overdue, Cancelled).</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A JSON string containing matching invoice items.</returns>
    [Description("Lists invoices with optional status filter (Draft, Sent, Paid, Overdue, Cancelled).")]
    public async Task<string> ListInvoices(
        [Description("Optional status filter: Draft, Sent, Paid, Overdue, Cancelled")] string? status = null,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Tool invoked: ListInvoices with status filter '{Status}'", status);

        InvoiceStatus? filterStatus = null;
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<InvoiceStatus>(status.Trim(), true, out var parsed))
        {
            filterStatus = parsed;
        }

        var query = new GetInvoicesQuery(new InvoiceSearchParameters(Status: filterStatus, Page: 1, PageSize: 50));
        var result = await dispatcher.QueryAsync(query, cancellationToken);

        if (result.IsFailure || result.Value is null)
        {
            return JsonSerializer.Serialize(new { success = false, error = "Unable to list invoices." }, JsonOptions);
        }

        return JsonSerializer.Serialize(result.Value, JsonOptions);
    }
}
