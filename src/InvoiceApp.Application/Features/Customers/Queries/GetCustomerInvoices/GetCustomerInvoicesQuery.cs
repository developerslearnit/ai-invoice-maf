using InvoiceApp.Application.Common.Messaging;
using InvoiceApp.Domain.Common;
using InvoiceApp.Domain.Errors;
using InvoiceApp.Domain.Models;
using InvoiceApp.Domain.Repositories;

namespace InvoiceApp.Application.Features.Customers.Queries.GetCustomerInvoices;

public sealed record GetCustomerInvoicesQuery(Guid CustomerId) : IQuery<Result<IReadOnlyList<InvoiceListItemDto>>>;

public sealed class GetCustomerInvoicesQueryHandler : IQueryHandler<GetCustomerInvoicesQuery, Result<IReadOnlyList<InvoiceListItemDto>>>
{
    private readonly ICustomerReadRepository _customerReadRepository;
    private readonly IInvoiceReadRepository _invoiceReadRepository;

    public GetCustomerInvoicesQueryHandler(
        ICustomerReadRepository customerReadRepository,
        IInvoiceReadRepository invoiceReadRepository)
    {
        _customerReadRepository = customerReadRepository;
        _invoiceReadRepository = invoiceReadRepository;
    }

    public async Task<Result<IReadOnlyList<InvoiceListItemDto>>> HandleAsync(
        GetCustomerInvoicesQuery query,
        CancellationToken cancellationToken = default)
    {
        var customerExists = await _customerReadRepository.ExistsAsync(query.CustomerId, cancellationToken);
        if (!customerExists)
        {
            return Result.Failure<IReadOnlyList<InvoiceListItemDto>>(CustomerErrors.NotFound(query.CustomerId));
        }

        var invoices = await _invoiceReadRepository.GetCustomerInvoicesAsync(query.CustomerId, cancellationToken);
        return Result.Success(invoices);
    }
}
