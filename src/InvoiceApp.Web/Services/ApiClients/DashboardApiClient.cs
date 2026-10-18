using InvoiceApp.Web.Models.Api;

namespace InvoiceApp.Web.Services.ApiClients;

public sealed class DashboardApiClient : BaseApiClient, IDashboardApiClient
{
    public DashboardApiClient(HttpClient httpClient, ILogger<DashboardApiClient> logger)
        : base(httpClient, logger)
    {
    }

    public async Task<ApiResponse<InvoiceSummaryApiModel>> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        return await GetAsync<InvoiceSummaryApiModel>("/api/invoices/summary", cancellationToken);
    }

    public async Task<ApiResponse<IReadOnlyList<InvoiceListItemApiModel>>> GetRecentInvoicesAsync(int count = 5, CancellationToken cancellationToken = default)
    {
        var response = await GetAsync<PagedListApiModel<InvoiceListItemApiModel>>($"/api/invoices?page=1&pageSize={count}", cancellationToken);
        if (response.IsSuccess && response.Data is not null)
        {
            return ApiResponse<IReadOnlyList<InvoiceListItemApiModel>>.Success(response.Data.Items, response.StatusCode);
        }

        return ApiResponse<IReadOnlyList<InvoiceListItemApiModel>>.Failure(response.ErrorMessage ?? "Failed to load recent invoices.", response.StatusCode);
    }

    public async Task<ApiResponse<IReadOnlyList<InvoiceListItemApiModel>>> GetOverdueInvoicesAsync(CancellationToken cancellationToken = default)
    {
        return await GetAsync<IReadOnlyList<InvoiceListItemApiModel>>("/api/invoices/overdue", cancellationToken);
    }
}
