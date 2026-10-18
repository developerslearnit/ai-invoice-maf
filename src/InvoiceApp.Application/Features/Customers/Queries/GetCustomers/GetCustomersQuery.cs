using InvoiceApp.Application.Common.Messaging;
using InvoiceApp.Domain.Common;
using InvoiceApp.Domain.Models;
using InvoiceApp.Domain.Repositories;

namespace InvoiceApp.Application.Features.Customers.Queries.GetCustomers;

public sealed record GetCustomersQuery() : IQuery<Result<IReadOnlyList<CustomerDto>>>;

public sealed class GetCustomersQueryHandler : IQueryHandler<GetCustomersQuery, Result<IReadOnlyList<CustomerDto>>>
{
    private readonly ICustomerReadRepository _customerReadRepository;

    public GetCustomersQueryHandler(ICustomerReadRepository customerReadRepository)
    {
        _customerReadRepository = customerReadRepository;
    }

    public async Task<Result<IReadOnlyList<CustomerDto>>> HandleAsync(
        GetCustomersQuery query,
        CancellationToken cancellationToken = default)
    {
        var customers = await _customerReadRepository.GetAllAsync(cancellationToken);
        return Result.Success(customers);
    }
}
