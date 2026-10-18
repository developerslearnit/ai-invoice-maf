using FluentValidation;
using InvoiceApp.Application.Common.Messaging;
using InvoiceApp.Domain.Common;
using InvoiceApp.Domain.Entities;
using InvoiceApp.Domain.Errors;
using InvoiceApp.Domain.Repositories;
using InvoiceApp.Domain.UnitOfWork;

namespace InvoiceApp.Application.Features.Customers.Commands.CreateCustomer;

public sealed record CreateCustomerCommand(
    string Name,
    string Email,
    string? Phone = null,
    string? Address = null) : ICommand<Result<Guid>>;

public sealed class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Customer name is required.")
            .MaximumLength(200).WithMessage("Customer name cannot exceed 200 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Customer email is required.")
            .EmailAddress().WithMessage("Customer email must be a valid email address.")
            .MaximumLength(320).WithMessage("Customer email cannot exceed 320 characters.");

        RuleFor(x => x.Phone)
            .MaximumLength(50).WithMessage("Phone cannot exceed 50 characters.")
            .When(x => !string.IsNullOrEmpty(x.Phone));

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("Address cannot exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.Address));
    }
}

public sealed class CreateCustomerCommandHandler : ICommandHandler<CreateCustomerCommand, Result<Guid>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICustomerReadRepository _customerReadRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerCommandHandler(
        ICustomerRepository customerRepository,
        ICustomerReadRepository customerReadRepository,
        IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _customerReadRepository = customerReadRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> HandleAsync(
        CreateCustomerCommand command,
        CancellationToken cancellationToken = default)
    {
        var isEmailUnique = await _customerReadRepository.IsEmailUniqueAsync(
            command.Email,
            excludeCustomerId: null,
            cancellationToken);

        if (!isEmailUnique)
        {
            return Result.Failure<Guid>(CustomerErrors.EmailNotUnique(command.Email));
        }

        var customerResult = Customer.Create(
            command.Name,
            command.Email,
            command.Phone,
            command.Address);

        if (customerResult.IsFailure)
        {
            return Result.Failure<Guid>(customerResult.Error);
        }

        var customer = customerResult.Value;

        await _customerRepository.AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(customer.Id);
    }
}
