using InvoiceApp.Web.Models.Api;

namespace InvoiceApp.Web.Services.ApiClients;

public interface ICustomerApiClient
{
    Task<ApiResponse<IReadOnlyList<CustomerApiModel>>> GetCustomersAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<CustomerApiModel>> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiResponse<Guid>> CreateCustomerAsync(CreateCustomerApiRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> UpdateCustomerAsync(Guid id, UpdateCustomerApiRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<IReadOnlyList<InvoiceListItemApiModel>>> GetCustomerInvoicesAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<ApiResponse<CustomerSummaryApiModel>> GetCustomerSummaryAsync(Guid customerId, CancellationToken cancellationToken = default);
}
