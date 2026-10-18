using System.Text;
using InvoiceApp.Web.Models.Api;

namespace InvoiceApp.Web.Services.ApiClients;

public sealed class InvoiceApiClient : BaseApiClient, IInvoiceApiClient
{
    public InvoiceApiClient(HttpClient httpClient, ILogger<InvoiceApiClient> logger)
        : base(httpClient, logger)
    {
    }

    public async Task<ApiResponse<PagedListApiModel<InvoiceListItemApiModel>>> GetInvoicesAsync(
        InvoiceFilterParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var sb = new StringBuilder("/api/invoices?");
        sb.Append($"page={parameters.Page}&pageSize={parameters.PageSize}");

        if (parameters.Status.HasValue)
        {
            sb.Append($"&status={parameters.Status.Value}");
        }

        if (parameters.CustomerId.HasValue && parameters.CustomerId.Value != Guid.Empty)
        {
            sb.Append($"&customerId={parameters.CustomerId.Value}");
        }

        if (parameters.FromDate.HasValue)
        {
            sb.Append($"&fromDate={parameters.FromDate.Value:yyyy-MM-dd}");
        }

        if (parameters.ToDate.HasValue)
        {
            sb.Append($"&toDate={parameters.ToDate.Value:yyyy-MM-dd}");
        }

        if (parameters.MinimumAmount.HasValue)
        {
            sb.Append($"&minimumAmount={parameters.MinimumAmount.Value}");
        }

        if (parameters.MaximumAmount.HasValue)
        {
            sb.Append($"&maximumAmount={parameters.MaximumAmount.Value}");
        }

        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            sb.Append($"&search={Uri.EscapeDataString(parameters.Search.Trim())}");
        }

        return await GetAsync<PagedListApiModel<InvoiceListItemApiModel>>(sb.ToString(), cancellationToken);
    }

    public async Task<ApiResponse<InvoiceApiModel>> GetInvoiceByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await GetAsync<InvoiceApiModel>($"/api/invoices/{id}", cancellationToken);
    }

    public async Task<ApiResponse<InvoiceApiModel>> GetInvoiceByNumberAsync(string invoiceNumber, CancellationToken cancellationToken = default)
    {
        return await GetAsync<InvoiceApiModel>($"/api/invoices/by-number/{Uri.EscapeDataString(invoiceNumber)}", cancellationToken);
    }

    public async Task<ApiResponse<Guid>> CreateInvoiceAsync(CreateInvoiceApiRequest request, CancellationToken cancellationToken = default)
    {
        var response = await PostAsJsonAsync<CreateInvoiceApiRequest, InvoiceCreatedResponse>("/api/invoices", request, cancellationToken);
        if (response.IsSuccess && response.Data is not null)
        {
            return ApiResponse<Guid>.Success(response.Data.Id, response.StatusCode);
        }

        return ApiResponse<Guid>.Failure(response.ErrorMessage ?? "Failed to create invoice.", response.StatusCode, response.ValidationErrors);
    }

    public async Task<ApiResponse<bool>> UpdateInvoiceAsync(Guid id, UpdateInvoiceApiRequest request, CancellationToken cancellationToken = default)
    {
        return await PutAsJsonAsync($"/api/invoices/{id}", request, cancellationToken);
    }

    public async Task<ApiResponse<bool>> DeleteInvoiceAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DeleteAsync($"/api/invoices/{id}", cancellationToken);
    }

    public async Task<ApiResponse<bool>> UpdateInvoiceStatusAsync(Guid id, InvoiceStatus status, CancellationToken cancellationToken = default)
    {
        var request = new UpdateInvoiceStatusApiRequest(status);
        return await PatchAsJsonAsync($"/api/invoices/{id}/status", request, cancellationToken);
    }

    public async Task<ApiResponse<IReadOnlyList<InvoiceListItemApiModel>>> GetOverdueInvoicesAsync(CancellationToken cancellationToken = default)
    {
        return await GetAsync<IReadOnlyList<InvoiceListItemApiModel>>("/api/invoices/overdue", cancellationToken);
    }

    public async Task<ApiResponse<InvoiceSummaryApiModel>> GetInvoiceSummaryAsync(CancellationToken cancellationToken = default)
    {
        return await GetAsync<InvoiceSummaryApiModel>("/api/invoices/summary", cancellationToken);
    }

    private sealed record InvoiceCreatedResponse(Guid Id);
}
