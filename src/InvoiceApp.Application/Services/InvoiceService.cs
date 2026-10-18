using InvoiceApp.Application.Common.Messaging;
using InvoiceApp.Application.Features.Customers.Queries.GetCustomerInvoices;
using InvoiceApp.Application.Features.Customers.Queries.GetCustomerSummary;
using InvoiceApp.Application.Features.Invoices.Commands.UpdateInvoiceStatus;
using InvoiceApp.Application.Features.Invoices.Queries.GetInvoiceById;
using InvoiceApp.Application.Features.Invoices.Queries.GetInvoiceByNumber;
using InvoiceApp.Application.Features.Invoices.Queries.GetInvoiceSummary;
using InvoiceApp.Application.Features.Invoices.Queries.GetOverdueInvoices;
using InvoiceApp.Domain.Common;
using InvoiceApp.Domain.Enums;
using InvoiceApp.Domain.Models;
using InvoiceApp.Domain.Repositories;

namespace InvoiceApp.Application.Services;

public sealed class InvoiceService : IInvoiceService
{
    private readonly IDispatcher _dispatcher;
    private readonly IInvoiceReadRepository _invoiceReadRepository;

    public InvoiceService(
        IDispatcher dispatcher,
        IInvoiceReadRepository invoiceReadRepository)
    {
        _dispatcher = dispatcher;
        _invoiceReadRepository = invoiceReadRepository;
    }

    public Task<Result<InvoiceDto>> GetInvoiceByNumberAsync(string invoiceNumber, CancellationToken cancellationToken = default)
    {
        return _dispatcher.QueryAsync(new GetInvoiceByNumberQuery(invoiceNumber), cancellationToken);
    }

    public Task<Result<InvoiceDto>> GetInvoiceByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dispatcher.QueryAsync(new GetInvoiceByIdQuery(id), cancellationToken);
    }

    public async Task<Result<IReadOnlyList<InvoiceListItemDto>>> GetInvoicesByStatusAsync(InvoiceStatus status, CancellationToken cancellationToken = default)
    {
        var invoices = await _invoiceReadRepository.GetByStatusAsync(status, cancellationToken);
        return Result.Success(invoices);
    }

    public Task<Result<IReadOnlyList<InvoiceListItemDto>>> GetCustomerInvoicesAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return _dispatcher.QueryAsync(new GetCustomerInvoicesQuery(customerId), cancellationToken);
    }

    public Task<Result<IReadOnlyList<InvoiceListItemDto>>> GetOverdueInvoicesAsync(CancellationToken cancellationToken = default)
    {
        return _dispatcher.QueryAsync(new GetOverdueInvoicesQuery(), cancellationToken);
    }

    public Task<Result<InvoiceSummaryDto>> GetInvoiceSummaryAsync(CancellationToken cancellationToken = default)
    {
        return _dispatcher.QueryAsync(new GetInvoiceSummaryQuery(), cancellationToken);
    }

    public Task<Result<CustomerSummaryDto>> GetCustomerSummaryAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return _dispatcher.QueryAsync(new GetCustomerSummaryQuery(customerId), cancellationToken);
    }

    public Task<Result> UpdateInvoiceStatusAsync(Guid id, InvoiceStatus status, CancellationToken cancellationToken = default)
    {
        return _dispatcher.SendAsync(new UpdateInvoiceStatusCommand(id, status), cancellationToken);
    }
}
