using InvoiceApp.Domain.Common;
using InvoiceApp.Domain.Enums;
using InvoiceApp.Domain.Models;

namespace InvoiceApp.Application.Services;

/// <summary>
/// Application service providing core invoice operations.
/// Designed for reuse by both API endpoints and future Microsoft Agent Framework tool bindings.
/// Contains no HTTP-specific logic and returns strongly typed Result models.
/// </summary>
public interface IInvoiceService
{
    Task<Result<InvoiceDto>> GetInvoiceByNumberAsync(string invoiceNumber, CancellationToken cancellationToken = default);
    Task<Result<InvoiceDto>> GetInvoiceByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<InvoiceListItemDto>>> GetInvoicesByStatusAsync(InvoiceStatus status, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<InvoiceListItemDto>>> GetCustomerInvoicesAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<InvoiceListItemDto>>> GetOverdueInvoicesAsync(CancellationToken cancellationToken = default);
    Task<Result<InvoiceSummaryDto>> GetInvoiceSummaryAsync(CancellationToken cancellationToken = default);
    Task<Result<CustomerSummaryDto>> GetCustomerSummaryAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<Result> UpdateInvoiceStatusAsync(Guid id, InvoiceStatus status, CancellationToken cancellationToken = default);
}
