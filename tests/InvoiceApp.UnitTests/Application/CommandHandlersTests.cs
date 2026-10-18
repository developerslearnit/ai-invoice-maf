using InvoiceApp.Application.DTOs;
using InvoiceApp.Application.Features.Customers.Commands.CreateCustomer;
using InvoiceApp.Application.Features.Invoices.Commands.CreateInvoice;
using InvoiceApp.Application.Features.Invoices.Commands.DeleteInvoice;
using InvoiceApp.Application.Features.Invoices.Commands.UpdateInvoiceStatus;
using InvoiceApp.Domain.Entities;
using InvoiceApp.Domain.Enums;
using InvoiceApp.Domain.Errors;
using InvoiceApp.Domain.Models;
using InvoiceApp.Domain.Repositories;
using InvoiceApp.Domain.Services;
using InvoiceApp.Domain.UnitOfWork;
using Xunit;

namespace InvoiceApp.UnitTests.Application;

public sealed class CommandHandlersTests
{
    private static readonly DateOnly Today = new(2026, 10, 14);

    [Fact]
    public async Task CreateCustomerHandler_WhenEmailNotUnique_ShouldReturnConflict()
    {
        // Arrange
        var fakeRepo = new FakeCustomerRepository();
        var fakeReadRepo = new FakeCustomerReadRepository { IsEmailUniqueResult = false };
        var fakeUow = new FakeUnitOfWork();

        var handler = new CreateCustomerCommandHandler(fakeRepo, fakeReadRepo, fakeUow);
        var command = new CreateCustomerCommand("Acme", "acme@example.com");

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(CustomerErrors.EmailNotUnique("acme@example.com").Code, result.Error.Code);
        Assert.Equal(0, fakeUow.SaveCount);
    }

    [Fact]
    public async Task CreateCustomerHandler_WhenValid_ShouldPersistAndReturnId()
    {
        // Arrange
        var fakeRepo = new FakeCustomerRepository();
        var fakeReadRepo = new FakeCustomerReadRepository { IsEmailUniqueResult = true };
        var fakeUow = new FakeUnitOfWork();

        var handler = new CreateCustomerCommandHandler(fakeRepo, fakeReadRepo, fakeUow);
        var command = new CreateCustomerCommand("Acme", "acme@example.com");

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);
        Assert.Single(fakeRepo.AddedCustomers);
        Assert.Equal(1, fakeUow.SaveCount);
    }

    [Fact]
    public async Task CreateInvoiceHandler_WhenCustomerDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var fakeRepo = new FakeInvoiceRepository();
        var fakeCustomerReadRepo = new FakeCustomerReadRepository { ExistsResult = false };
        var fakeNumGen = new FakeNumberGenerator("INV-000001");
        var fakeUow = new FakeUnitOfWork();

        var handler = new CreateInvoiceCommandHandler(fakeRepo, fakeCustomerReadRepo, fakeNumGen, fakeUow);
        var command = new CreateInvoiceCommand(
            Guid.NewGuid(),
            Today,
            Today.AddDays(15),
            null,
            [new CreateInvoiceItemRequest("Item 1", 1, 100m)]);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(CustomerErrors.NotFound(command.CustomerId).Code, result.Error.Code);
    }

    [Fact]
    public async Task DeleteInvoiceHandler_WhenInvoiceIsNotDraft_ShouldFail()
    {
        // Arrange
        var invoice = Invoice.Create("INV-000001", Guid.NewGuid(), Today, Today.AddDays(10), null, [("Item", 1, 100m)]).Value;
        invoice.TransitionTo(InvoiceStatus.Sent);

        var fakeRepo = new FakeInvoiceRepository { ExistingInvoice = invoice };
        var fakeUow = new FakeUnitOfWork();

        var handler = new DeleteInvoiceCommandHandler(fakeRepo, fakeUow);

        // Act
        var result = await handler.HandleAsync(new DeleteInvoiceCommand(invoice.Id));

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(InvoiceErrors.CannotDeleteNonDraft.Code, result.Error.Code);
        Assert.Null(fakeRepo.RemovedInvoice);
    }

    [Fact]
    public async Task DeleteInvoiceHandler_WhenInvoiceIsDraft_ShouldSucceed()
    {
        // Arrange
        var invoice = Invoice.Create("INV-000001", Guid.NewGuid(), Today, Today.AddDays(10), null, [("Item", 1, 100m)]).Value;

        var fakeRepo = new FakeInvoiceRepository { ExistingInvoice = invoice };
        var fakeUow = new FakeUnitOfWork();

        var handler = new DeleteInvoiceCommandHandler(fakeRepo, fakeUow);

        // Act
        var result = await handler.HandleAsync(new DeleteInvoiceCommand(invoice.Id));

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(fakeRepo.RemovedInvoice);
        Assert.Equal(1, fakeUow.SaveCount);
    }

    [Fact]
    public async Task UpdateInvoiceStatusHandler_WhenInvalidTransition_ShouldReturnError()
    {
        // Arrange
        var invoice = Invoice.Create("INV-000001", Guid.NewGuid(), Today, Today.AddDays(10), null, [("Item", 1, 100m)]).Value;
        var fakeRepo = new FakeInvoiceRepository { ExistingInvoice = invoice };
        var fakeUow = new FakeUnitOfWork();

        var handler = new UpdateInvoiceStatusCommandHandler(fakeRepo, fakeUow);

        // Act - Attempt Draft -> Paid (invalid!)
        var result = await handler.HandleAsync(new UpdateInvoiceStatusCommand(invoice.Id, InvoiceStatus.Paid));

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(InvoiceErrors.InvalidStatusTransition(InvoiceStatus.Draft, InvoiceStatus.Paid).Code, result.Error.Code);
    }
}

// In-memory test doubles
internal sealed class FakeCustomerRepository : ICustomerRepository
{
    public List<Customer> AddedCustomers { get; } = [];

    public Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(AddedCustomers.FirstOrDefault(c => c.Id == id));

    public Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        Task.FromResult(AddedCustomers.FirstOrDefault(c => c.Email == email));

    public Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        AddedCustomers.Add(customer);
        return Task.CompletedTask;
    }

    public void Update(Customer customer) { }
}

internal sealed class FakeCustomerReadRepository : ICustomerReadRepository
{
    public bool ExistsResult { get; set; } = true;
    public bool IsEmailUniqueResult { get; set; } = true;

    public Task<CustomerDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<CustomerDto?>(null);
    public Task<IReadOnlyList<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<CustomerDto>>([]);
    public Task<CustomerSummaryDto?> GetSummaryAsync(Guid customerId, CancellationToken cancellationToken = default) => Task.FromResult<CustomerSummaryDto?>(null);
    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(ExistsResult);
    public Task<bool> IsEmailUniqueAsync(string email, Guid? excludeCustomerId = null, CancellationToken cancellationToken = default) => Task.FromResult(IsEmailUniqueResult);
}

internal sealed class FakeInvoiceRepository : IInvoiceRepository
{
    public Invoice? ExistingInvoice { get; set; }
    public Invoice? RemovedInvoice { get; set; }

    public Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(ExistingInvoice);
    public Task<Invoice?> GetByNumberAsync(string invoiceNumber, CancellationToken cancellationToken = default) => Task.FromResult(ExistingInvoice);
    public Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public void Remove(Invoice invoice) => RemovedInvoice = invoice;
}

internal sealed class FakeNumberGenerator : IInvoiceNumberGenerator
{
    private readonly string _number;
    public FakeNumberGenerator(string number) => _number = number;
    public Task<string> NextInvoiceNumberAsync(CancellationToken cancellationToken = default) => Task.FromResult(_number);
}

internal sealed class FakeUnitOfWork : IUnitOfWork
{
    public int SaveCount { get; private set; }
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SaveCount++;
        return Task.FromResult(1);
    }
}
