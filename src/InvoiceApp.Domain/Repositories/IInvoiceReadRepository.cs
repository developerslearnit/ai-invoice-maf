using InvoiceApp.Domain.Enums;
using InvoiceApp.Domain.Models;

namespace InvoiceApp.Domain.Repositories;

public interface IInvoiceReadRepository
{
    Task<InvoiceDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<InvoiceDto?> GetByNumberAsync(string invoiceNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InvoiceListItemDto>> GetByStatusAsync(InvoiceStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InvoiceListItemDto>> GetCustomerInvoicesAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InvoiceListItemDto>> GetOverdueInvoicesAsync(DateOnly currentDate, CancellationToken cancellationToken = default);
    Task<InvoiceSummaryDto> GetSummaryAsync(DateOnly currentDate, CancellationToken cancellationToken = default);
    Task<PagedList<InvoiceListItemDto>> SearchAsync(InvoiceSearchParameters parameters, CancellationToken cancellationToken = default);
}
