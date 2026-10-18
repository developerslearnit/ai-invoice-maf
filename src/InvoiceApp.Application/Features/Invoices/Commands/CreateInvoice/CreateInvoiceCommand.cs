using FluentValidation;
using InvoiceApp.Application.Common.Messaging;
using InvoiceApp.Application.DTOs;
using InvoiceApp.Domain.Common;
using InvoiceApp.Domain.Entities;
using InvoiceApp.Domain.Errors;
using InvoiceApp.Domain.Repositories;
using InvoiceApp.Domain.Services;
using InvoiceApp.Domain.UnitOfWork;

namespace InvoiceApp.Application.Features.Invoices.Commands.CreateInvoice;

public sealed record CreateInvoiceCommand(
    Guid CustomerId,
    DateOnly IssueDate,
    DateOnly DueDate,
    string? Notes,
    IReadOnlyCollection<CreateInvoiceItemRequest> Items) : ICommand<Result<Guid>>;

public sealed class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("CustomerId is required.");

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

public sealed class CreateInvoiceCommandHandler : ICommandHandler<CreateInvoiceCommand, Result<Guid>>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ICustomerReadRepository _customerReadRepository;
    private readonly IInvoiceNumberGenerator _numberGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public CreateInvoiceCommandHandler(
        IInvoiceRepository invoiceRepository,
        ICustomerReadRepository customerReadRepository,
        IInvoiceNumberGenerator numberGenerator,
        IUnitOfWork unitOfWork)
    {
        _invoiceRepository = invoiceRepository;
        _customerReadRepository = customerReadRepository;
        _numberGenerator = numberGenerator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> HandleAsync(
        CreateInvoiceCommand command,
        CancellationToken cancellationToken = default)
    {
        var customerExists = await _customerReadRepository.ExistsAsync(command.CustomerId, cancellationToken);
        if (!customerExists)
        {
            return Result.Failure<Guid>(CustomerErrors.NotFound(command.CustomerId));
        }

        var invoiceNumber = await _numberGenerator.NextInvoiceNumberAsync(cancellationToken);

        var items = command.Items.Select(x => (x.Description, x.Quantity, x.UnitPrice));

        var invoiceResult = Invoice.Create(
            invoiceNumber,
            command.CustomerId,
            command.IssueDate,
            command.DueDate,
            command.Notes,
            items);

        if (invoiceResult.IsFailure)
        {
            return Result.Failure<Guid>(invoiceResult.Error);
        }

        var invoice = invoiceResult.Value;

        await _invoiceRepository.AddAsync(invoice, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(invoice.Id);
    }
}
