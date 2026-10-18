using InvoiceApp.Application.Common.Messaging;
using InvoiceApp.Domain.Common;
using InvoiceApp.Domain.Models;
using InvoiceApp.Domain.Repositories;

namespace InvoiceApp.Application.Features.Invoices.Queries.GetInvoices;

public sealed record GetInvoicesQuery(InvoiceSearchParameters Parameters) : IQuery<Result<PagedList<InvoiceListItemDto>>>;

public sealed class GetInvoicesQueryHandler : IQueryHandler<GetInvoicesQuery, Result<PagedList<InvoiceListItemDto>>>
{
    private readonly IInvoiceReadRepository _invoiceReadRepository;

    public GetInvoicesQueryHandler(IInvoiceReadRepository invoiceReadRepository)
    {
        _invoiceReadRepository = invoiceReadRepository;
    }

    public async Task<Result<PagedList<InvoiceListItemDto>>> HandleAsync(
        GetInvoicesQuery query,
        CancellationToken cancellationToken = default)
    {
        var result = await _invoiceReadRepository.SearchAsync(query.Parameters, cancellationToken);
        return Result.Success(result);
    }
}
