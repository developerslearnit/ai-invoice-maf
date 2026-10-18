using InvoiceApp.Application.Common.Messaging;
using InvoiceApp.Domain.Common;
using InvoiceApp.Domain.Errors;
using InvoiceApp.Domain.Repositories;
using InvoiceApp.Domain.UnitOfWork;

namespace InvoiceApp.Application.Features.Invoices.Commands.DeleteInvoice;

public sealed record DeleteInvoiceCommand(Guid Id) : ICommand;

public sealed class DeleteInvoiceCommandHandler : ICommandHandler<DeleteInvoiceCommand>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteInvoiceCommandHandler(
        IInvoiceRepository invoiceRepository,
        IUnitOfWork unitOfWork)
    {
        _invoiceRepository = invoiceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(
        DeleteInvoiceCommand command,
        CancellationToken cancellationToken = default)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(command.Id, cancellationToken);
        if (invoice is null)
        {
            return Result.Failure(InvoiceErrors.NotFound(command.Id));
        }

        if (!invoice.CanBeDeleted)
        {
            return Result.Failure(InvoiceErrors.CannotDeleteNonDraft);
        }

        _invoiceRepository.Remove(invoice);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
