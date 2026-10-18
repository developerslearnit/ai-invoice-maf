using FluentValidation;
using InvoiceApp.Application.Common.Messaging;
using InvoiceApp.Application.DTOs;
using InvoiceApp.Domain.Common;
using InvoiceApp.Domain.Errors;
using InvoiceApp.Domain.Repositories;
using InvoiceApp.Domain.UnitOfWork;

namespace InvoiceApp.Application.Features.Invoices.Commands.UpdateInvoice;

public sealed record UpdateInvoiceCommand(
    Guid Id,
    DateOnly IssueDate,
    DateOnly DueDate,
    string? Notes,
    IReadOnlyCollection<CreateInvoiceItemRequest> Items) : ICommand;

public sealed class UpdateInvoiceCommandValidator : AbstractValidator<UpdateInvoiceCommand>
{
    public UpdateInvoiceCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Invoice ID is required.");

        RuleFor(x => x.IssueDate)
            .NotEmpty().WithMessage("IssueDate is required.");

        RuleFor(x => x.DueDate)
            .NotEmpty().WithMessage("DueDate is required.")
            .GreaterThanOrEqualTo(x => x.IssueDate)
            .WithMessage("Due date cannot be earlier than issue date.");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("An invoice must contain at least one item.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.Description)
                .NotEmpty().WithMessage("Invoice item description is required.")
                .MaximumLength(300).WithMessage("Invoice item description cannot exceed 300 characters.");

            item.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.");

            item.RuleFor(i => i.UnitPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Unit price must be greater than or equal to zero.");
        });

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}

public sealed class UpdateInvoiceCommandHandler : ICommandHandler<UpdateInvoiceCommand>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateInvoiceCommandHandler(
        IInvoiceRepository invoiceRepository,
        IUnitOfWork unitOfWork)
    {
        _invoiceRepository = invoiceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(
        UpdateInvoiceCommand command,
        CancellationToken cancellationToken = default)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(command.Id, cancellationToken);
        if (invoice is null)
        {
            return Result.Failure(InvoiceErrors.NotFound(command.Id));
        }

        var items = command.Items.Select(x => (x.Description, x.Quantity, x.UnitPrice));

        var updateResult = invoice.Update(
            command.IssueDate,
            command.DueDate,
            command.Notes,
            items);

        if (updateResult.IsFailure)
        {
            return updateResult;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
