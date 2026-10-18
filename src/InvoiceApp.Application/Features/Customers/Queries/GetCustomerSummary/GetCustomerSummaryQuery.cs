using InvoiceApp.Application.Common.Messaging;
using InvoiceApp.Domain.Common;
using InvoiceApp.Domain.Errors;
using InvoiceApp.Domain.Models;
using InvoiceApp.Domain.Repositories;

namespace InvoiceApp.Application.Features.Customers.Queries.GetCustomerSummary;

public sealed record GetCustomerSummaryQuery(Guid CustomerId) : IQuery<Result<CustomerSummaryDto>>;

public sealed class GetCustomerSummaryQueryHandler : IQueryHandler<GetCustomerSummaryQuery, Result<CustomerSummaryDto>>
{
    private readonly ICustomerReadRepository _customerReadRepository;

    public GetCustomerSummaryQueryHandler(ICustomerReadRepository customerReadRepository)
    {
        _customerReadRepository = customerReadRepository;
    }

    public async Task<Result<CustomerSummaryDto>> HandleAsync(
        GetCustomerSummaryQuery query,
        CancellationToken cancellationToken = default)
    {
        var exists = await _customerReadRepository.ExistsAsync(query.CustomerId, cancellationToken);
        if (!exists)
        {
            return Result.Failure<CustomerSummaryDto>(CustomerErrors.NotFound(query.CustomerId));
        }

        var summary = await _customerReadRepository.GetSummaryAsync(query.CustomerId, cancellationToken);
        if (summary is null)
        {
            return Result.Failure<CustomerSummaryDto>(CustomerErrors.NotFound(query.CustomerId));
        }

        return Result.Success(summary);
    }
}
