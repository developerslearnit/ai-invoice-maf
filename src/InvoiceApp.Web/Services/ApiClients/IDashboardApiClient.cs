using InvoiceApp.Web.Models.Api;

namespace InvoiceApp.Web.Services.ApiClients;

public interface IDashboardApiClient
{
    Task<ApiResponse<InvoiceSummaryApiModel>> GetSummaryAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<IReadOnlyList<InvoiceListItemApiModel>>> GetRecentInvoicesAsync(int count = 5, CancellationToken cancellationToken = default);
    Task<ApiResponse<IReadOnlyList<InvoiceListItemApiModel>>> GetOverdueInvoicesAsync(CancellationToken cancellationToken = default);
}
