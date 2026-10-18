using InvoiceApp.Web.Models.Api;

namespace InvoiceApp.Web.Services.ApiClients;

public interface IInvoiceApiClient
{
    Task<ApiResponse<PagedListApiModel<InvoiceListItemApiModel>>> GetInvoicesAsync(InvoiceFilterParameters parameters, CancellationToken cancellationToken = default);
    Task<ApiResponse<InvoiceApiModel>> GetInvoiceByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiResponse<InvoiceApiModel>> GetInvoiceByNumberAsync(string invoiceNumber, CancellationToken cancellationToken = default);
    Task<ApiResponse<Guid>> CreateInvoiceAsync(CreateInvoiceApiRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> UpdateInvoiceAsync(Guid id, UpdateInvoiceApiRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> DeleteInvoiceAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> UpdateInvoiceStatusAsync(Guid id, InvoiceStatus status, CancellationToken cancellationToken = default);
    Task<ApiResponse<IReadOnlyList<InvoiceListItemApiModel>>> GetOverdueInvoicesAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<InvoiceSummaryApiModel>> GetInvoiceSummaryAsync(CancellationToken cancellationToken = default);
}
