using InvoiceApp.Application.Common.Messaging;
using InvoiceApp.Domain.Common;
using InvoiceApp.Domain.Models;
using InvoiceApp.Domain.Repositories;

namespace InvoiceApp.Application.Features.Invoices.Queries.GetInvoiceSummary;

public sealed record GetInvoiceSummaryQuery() : IQuery<Result<InvoiceSummaryDto>>;

public sealed class GetInvoiceSummaryQueryHandler : IQueryHandler<GetInvoiceSummaryQuery, Result<InvoiceSummaryDto>>
{
    private readonly IInvoiceReadRepository _invoiceReadRepository;

    public GetInvoiceSummaryQueryHandler(IInvoiceReadRepository invoiceReadRepository)
    {
        _invoiceReadRepository = invoiceReadRepository;
    }

    public async Task<Result<InvoiceSummaryDto>> HandleAsync(
        GetInvoiceSummaryQuery query,
        CancellationToken cancellationToken = default)
    {
        var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var summary = await _invoiceReadRepository.GetSummaryAsync(currentDate, cancellationToken);
        return Result.Success(summary);
    }
}
