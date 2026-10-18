using InvoiceApp.Domain.Models;

namespace InvoiceApp.Domain.Repositories;

public interface ICustomerReadRepository
{
    Task<CustomerDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CustomerSummaryDto?> GetSummaryAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> IsEmailUniqueAsync(string email, Guid? excludeCustomerId = null, CancellationToken cancellationToken = default);
}
