using InvoiceApp.Domain.Enums;

namespace InvoiceApp.Domain.Models;

public sealed record CustomerDto(
    Guid Id,
    string Name,
    string Email,
    string? Phone,
    string? Address,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);

public sealed record CustomerSummaryDto(
    Guid CustomerId,
    string CustomerName,
    int TotalInvoices,
    int PaidInvoices,
    int OutstandingInvoices,
    decimal TotalInvoiced,
    decimal TotalPaid,
    decimal TotalOutstanding);

public sealed record InvoiceItemDto(
    Guid Id,
    string Description,
    decimal Quantity,
    decimal UnitPrice,
    decimal LineTotal);

public sealed record InvoiceDto(
    Guid Id,
    string InvoiceNumber,
    CustomerDto Customer,
    DateOnly IssueDate,
    DateOnly DueDate,
    InvoiceStatus Status,
    string? Notes,
    decimal TotalAmount,
    IReadOnlyCollection<InvoiceItemDto> Items);

public sealed record InvoiceListItemDto(
    Guid Id,
    string InvoiceNumber,
    Guid CustomerId,
    string CustomerName,
    DateOnly IssueDate,
    DateOnly DueDate,
    InvoiceStatus Status,
    decimal TotalAmount);

public sealed record InvoiceSummaryDto(
    int TotalInvoices,
    int DraftInvoices,
    int SentInvoices,
    int PaidInvoices,
    int OverdueInvoices,
    decimal TotalInvoiceAmount,
    decimal TotalPaidAmount,
    decimal TotalOutstandingAmount);

public sealed record InvoiceSearchParameters(
    InvoiceStatus? Status = null,
    Guid? CustomerId = null,
    DateOnly? FromDate = null,
    DateOnly? ToDate = null,
    decimal? MinimumAmount = null,
    decimal? MaximumAmount = null,
    string? Search = null,
    int Page = 1,
    int PageSize = 20)
{
    public int Page { get; init; } = Page < 1 ? 1 : Page;
    public int PageSize { get; init; } = PageSize < 1 ? 20 : (PageSize > 100 ? 100 : PageSize);
}

public sealed record PagedList<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalCount)
{
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
}
