namespace InvoiceApp.Web.Models.Api;

public sealed record CustomerApiModel(
    Guid Id,
    string Name,
    string Email,
    string? Phone,
    string? Address,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);

public sealed record CustomerSummaryApiModel(
    Guid CustomerId,
    string CustomerName,
    int TotalInvoices,
    int PaidInvoices,
    int OutstandingInvoices,
    decimal TotalInvoiced,
    decimal TotalPaid,
    decimal TotalOutstanding);

public sealed record CreateCustomerApiRequest(
    string Name,
    string Email,
    string? Phone = null,
    string? Address = null);

public sealed record UpdateCustomerApiRequest(
    string Name,
    string Email,
    string? Phone = null,
    string? Address = null);
