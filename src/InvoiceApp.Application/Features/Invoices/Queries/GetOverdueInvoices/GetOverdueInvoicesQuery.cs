using InvoiceApp.Application.Common.Messaging;
using InvoiceApp.Domain.Common;
using InvoiceApp.Domain.Models;
using InvoiceApp.Domain.Repositories;

namespace InvoiceApp.Application.Features.Invoices.Queries.GetOverdueInvoices;

public sealed record GetOverdueInvoicesQuery() : IQuery<Result<IReadOnlyList<InvoiceListItemDto>>>;

public sealed class GetOverdueInvoicesQueryHandler : IQueryHandler<GetOverdueInvoicesQuery, Result<IReadOnlyList<InvoiceListItemDto>>>
{
    private readonly IInvoiceReadRepository _invoiceReadRepository;

    public GetOverdueInvoicesQueryHandler(IInvoiceReadRepository invoiceReadRepository)
    {
        _invoiceReadRepository = invoiceReadRepository;
    }

    public async Task<Result<IReadOnlyList<InvoiceListItemDto>>> HandleAsync(
        GetOverdueInvoicesQuery query,
        CancellationToken cancellationToken = default)
    {
        var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var overdueInvoices = await _invoiceReadRepository.GetOverdueInvoicesAsync(currentDate, cancellationToken);
        return Result.Success(overdueInvoices);
    }
}
