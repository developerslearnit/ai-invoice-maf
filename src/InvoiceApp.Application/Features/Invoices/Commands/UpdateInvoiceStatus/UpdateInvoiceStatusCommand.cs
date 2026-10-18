using FluentValidation;
using InvoiceApp.Application.Common.Messaging;
using InvoiceApp.Domain.Common;
using InvoiceApp.Domain.Enums;
using InvoiceApp.Domain.Errors;
using InvoiceApp.Domain.Repositories;
using InvoiceApp.Domain.UnitOfWork;

namespace InvoiceApp.Application.Features.Invoices.Commands.UpdateInvoiceStatus;

public sealed record UpdateInvoiceStatusCommand(
    Guid Id,
    InvoiceStatus Status) : ICommand;

public sealed class UpdateInvoiceStatusCommandValidator : AbstractValidator<UpdateInvoiceStatusCommand>
{
    public UpdateInvoiceStatusCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Invoice ID is required.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid invoice status.");
    }
}

public sealed class UpdateInvoiceStatusCommandHandler : ICommandHandler<UpdateInvoiceStatusCommand>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateInvoiceStatusCommandHandler(
        IInvoiceRepository invoiceRepository,
        IUnitOfWork unitOfWork)
    {
        _invoiceRepository = invoiceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(
        UpdateInvoiceStatusCommand command,
        CancellationToken cancellationToken = default)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(command.Id, cancellationToken);
        if (invoice is null)
        {
            return Result.Failure(InvoiceErrors.NotFound(command.Id));
        }

        var transitionResult = invoice.TransitionTo(command.Status);
        if (transitionResult.IsFailure)
        {
            return transitionResult;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
