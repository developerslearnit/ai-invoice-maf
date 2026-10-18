using InvoiceApp.Domain.Enums;

namespace InvoiceApp.Application.DTOs;

public sealed record CreateCustomerRequest(
    string Name,
    string Email,
    string? Phone = null,
    string? Address = null);

public sealed record UpdateCustomerRequest(
    string Name,
    string Email,
    string? Phone = null,
    string? Address = null);

public sealed record CreateInvoiceRequest(
    Guid CustomerId,
    DateOnly IssueDate,
    DateOnly DueDate,
    string? Notes,
    IReadOnlyCollection<CreateInvoiceItemRequest> Items);

public sealed record CreateInvoiceItemRequest(
    string Description,
    decimal Quantity,
    decimal UnitPrice);

public sealed record UpdateInvoiceRequest(
    DateOnly IssueDate,
    DateOnly DueDate,
    string? Notes,
    IReadOnlyCollection<CreateInvoiceItemRequest> Items);

public sealed record UpdateInvoiceStatusRequest(
    InvoiceStatus Status);
