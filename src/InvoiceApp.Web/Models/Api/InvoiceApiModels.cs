using System.Text.Json.Serialization;

namespace InvoiceApp.Web.Models.Api;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum InvoiceStatus
{
    Draft,
    Sent,
    Paid,
    Overdue,
    Cancelled
}

public sealed record InvoiceItemApiModel(
    Guid Id,
    string Description,
    decimal Quantity,
    decimal UnitPrice,
    decimal LineTotal);

public sealed record InvoiceApiModel(
    Guid Id,
    string InvoiceNumber,
    CustomerApiModel Customer,
    DateOnly IssueDate,
    DateOnly DueDate,
    InvoiceStatus Status,
    string? Notes,
    decimal TotalAmount,
    IReadOnlyCollection<InvoiceItemApiModel> Items);

public sealed record InvoiceListItemApiModel(
    Guid Id,
    string InvoiceNumber,
    Guid CustomerId,
    string CustomerName,
    DateOnly IssueDate,
    DateOnly DueDate,
    InvoiceStatus Status,
    decimal TotalAmount);

public sealed record InvoiceSummaryApiModel(
    int TotalInvoices,
    int DraftInvoices,
    int SentInvoices,
    int PaidInvoices,
    int OverdueInvoices,
    decimal TotalInvoiceAmount,
    decimal TotalPaidAmount,
    decimal TotalOutstandingAmount);

public sealed record CreateInvoiceItemApiRequest(
    string Description,
    decimal Quantity,
    decimal UnitPrice);

public sealed record CreateInvoiceApiRequest(
    Guid CustomerId,
    DateOnly IssueDate,
    DateOnly DueDate,
    string? Notes,
    IReadOnlyCollection<CreateInvoiceItemApiRequest> Items);

public sealed record UpdateInvoiceApiRequest(
    DateOnly IssueDate,
    DateOnly DueDate,
    string? Notes,
    IReadOnlyCollection<CreateInvoiceItemApiRequest> Items);

public sealed record UpdateInvoiceStatusApiRequest(
    InvoiceStatus Status);

public sealed record InvoiceFilterParameters(
    InvoiceStatus? Status = null,
    Guid? CustomerId = null,
    DateOnly? FromDate = null,
    DateOnly? ToDate = null,
    decimal? MinimumAmount = null,
    decimal? MaximumAmount = null,
    string? Search = null,
    int Page = 1,
    int PageSize = 20);
