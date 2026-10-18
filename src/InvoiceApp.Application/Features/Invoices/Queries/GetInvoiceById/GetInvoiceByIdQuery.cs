using InvoiceApp.Application.Common.Messaging;
using InvoiceApp.Domain.Common;
using InvoiceApp.Domain.Errors;
using InvoiceApp.Domain.Models;
using InvoiceApp.Domain.Repositories;

namespace InvoiceApp.Application.Features.Invoices.Queries.GetInvoiceById;

public sealed record GetInvoiceByIdQuery(Guid Id) : IQuery<Result<InvoiceDto>>;

public sealed class GetInvoiceByIdQueryHandler : IQueryHandler<GetInvoiceByIdQuery, Result<InvoiceDto>>
{
    private readonly IInvoiceReadRepository _invoiceReadRepository;

    public GetInvoiceByIdQueryHandler(IInvoiceReadRepository invoiceReadRepository)
    {
        _invoiceReadRepository = invoiceReadRepository;
    }

    public async Task<Result<InvoiceDto>> HandleAsync(
        GetInvoiceByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var invoice = await _invoiceReadRepository.GetByIdAsync(query.Id, cancellationToken);
        if (invoice is null)
        {
            return Result.Failure<InvoiceDto>(InvoiceErrors.NotFound(query.Id));
        }

        return Result.Success(invoice);
    }
}
