using InvoiceApp.Application.Common.Messaging;
using InvoiceApp.Domain.Common;
using InvoiceApp.Domain.Errors;
using InvoiceApp.Domain.Models;
using InvoiceApp.Domain.Repositories;

namespace InvoiceApp.Application.Features.Customers.Queries.GetCustomerById;

public sealed record GetCustomerByIdQuery(Guid Id) : IQuery<Result<CustomerDto>>;

public sealed class GetCustomerByIdQueryHandler : IQueryHandler<GetCustomerByIdQuery, Result<CustomerDto>>
{
    private readonly ICustomerReadRepository _customerReadRepository;

    public GetCustomerByIdQueryHandler(ICustomerReadRepository customerReadRepository)
    {
        _customerReadRepository = customerReadRepository;
    }

    public async Task<Result<CustomerDto>> HandleAsync(
        GetCustomerByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var customer = await _customerReadRepository.GetByIdAsync(query.Id, cancellationToken);
        if (customer is null)
        {
            return Result.Failure<CustomerDto>(CustomerErrors.NotFound(query.Id));
        }

        return Result.Success(customer);
    }
}
