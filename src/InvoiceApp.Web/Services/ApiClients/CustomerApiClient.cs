using InvoiceApp.Web.Models.Api;

namespace InvoiceApp.Web.Services.ApiClients;

public sealed class CustomerApiClient : BaseApiClient, ICustomerApiClient
{
    public CustomerApiClient(HttpClient httpClient, ILogger<CustomerApiClient> logger)
        : base(httpClient, logger)
    {
    }

    public async Task<ApiResponse<IReadOnlyList<CustomerApiModel>>> GetCustomersAsync(CancellationToken cancellationToken = default)
    {
        return await GetAsync<IReadOnlyList<CustomerApiModel>>("/api/customers", cancellationToken);
    }

    public async Task<ApiResponse<CustomerApiModel>> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await GetAsync<CustomerApiModel>($"/api/customers/{id}", cancellationToken);
    }

    public async Task<ApiResponse<Guid>> CreateCustomerAsync(CreateCustomerApiRequest request, CancellationToken cancellationToken = default)
    {
        var response = await PostAsJsonAsync<CreateCustomerApiRequest, CustomerCreatedResponse>("/api/customers", request, cancellationToken);
        if (response.IsSuccess && response.Data is not null)
        {
            return ApiResponse<Guid>.Success(response.Data.Id, response.StatusCode);
        }

        return ApiResponse<Guid>.Failure(response.ErrorMessage ?? "Failed to create customer.", response.StatusCode, response.ValidationErrors);
    }

    public async Task<ApiResponse<bool>> UpdateCustomerAsync(Guid id, UpdateCustomerApiRequest request, CancellationToken cancellationToken = default)
    {
        return await PutAsJsonAsync($"/api/customers/{id}", request, cancellationToken);
    }

    public async Task<ApiResponse<IReadOnlyList<InvoiceListItemApiModel>>> GetCustomerInvoicesAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await GetAsync<IReadOnlyList<InvoiceListItemApiModel>>($"/api/customers/{customerId}/invoices", cancellationToken);
    }

    public async Task<ApiResponse<CustomerSummaryApiModel>> GetCustomerSummaryAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await GetAsync<CustomerSummaryApiModel>($"/api/customers/{customerId}/summary", cancellationToken);
    }

    private sealed record CustomerCreatedResponse(Guid Id);
}
