using InvoiceApp.Application.Common.Messaging;
using InvoiceApp.Domain.Common;
using InvoiceApp.Domain.Errors;
using InvoiceApp.Domain.Models;
using InvoiceApp.Domain.Repositories;

namespace InvoiceApp.Application.Features.Invoices.Queries.GetInvoiceByNumber;

public sealed record GetInvoiceByNumberQuery(string InvoiceNumber) : IQuery<Result<InvoiceDto>>;

public sealed class GetInvoiceByNumberQueryHandler : IQueryHandler<GetInvoiceByNumberQuery, Result<InvoiceDto>>
{
    private readonly IInvoiceReadRepository _invoiceReadRepository;

    public GetInvoiceByNumberQueryHandler(IInvoiceReadRepository invoiceReadRepository)
    {
        _invoiceReadRepository = invoiceReadRepository;
    }

    public async Task<Result<InvoiceDto>> HandleAsync(
        GetInvoiceByNumberQuery query,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query.InvoiceNumber))
        {
            return Result.Failure<InvoiceDto>(InvoiceErrors.EmptyInvoiceNumber);
        }

        var invoice = await _invoiceReadRepository.GetByNumberAsync(query.InvoiceNumber.Trim(), cancellationToken);
        if (invoice is null)
        {
            return Result.Failure<InvoiceDto>(InvoiceErrors.NotFoundByNumber(query.InvoiceNumber));
        }

        return Result.Success(invoice);
    }
}
