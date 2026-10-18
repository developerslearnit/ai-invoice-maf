using InvoiceApp.Application.DTOs;
using InvoiceApp.Application.Features.Customers.Commands.CreateCustomer;
using InvoiceApp.Application.Features.Invoices.Commands.CreateInvoice;
using InvoiceApp.Application.Features.Invoices.Commands.UpdateInvoiceStatus;
using InvoiceApp.Domain.Enums;
using Xunit;

namespace InvoiceApp.UnitTests.Application;

public sealed class ValidatorsTests
{
    private static readonly DateOnly Today = new(2026, 10, 14);

    [Fact]
    public void CreateCustomerValidator_WhenValid_ShouldPass()
    {
        var validator = new CreateCustomerCommandValidator();
        var command = new CreateCustomerCommand("Contoso Ltd", "info@contoso.com", "+1-555-1234", "100 Microsoft Way");

        var result = validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("", "info@contoso.com")]
    [InlineData("Contoso", "invalid-email")]
    [InlineData("Contoso", "")]
    public void CreateCustomerValidator_WhenInvalid_ShouldFail(string name, string email)
    {
        var validator = new CreateCustomerCommandValidator();
        var command = new CreateCustomerCommand(name, email);

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void CreateInvoiceValidator_WhenValid_ShouldPass()
    {
        var validator = new CreateInvoiceCommandValidator();
        var command = new CreateInvoiceCommand(
            Guid.NewGuid(),
            Today,
            Today.AddDays(15),
            "Notes",
            [new CreateInvoiceItemRequest("Item 1", 2, 50m)]);

        var result = validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void CreateInvoiceValidator_WhenDueDateBeforeIssueDate_ShouldFail()
    {
        var validator = new CreateInvoiceCommandValidator();
        var command = new CreateInvoiceCommand(
            Guid.NewGuid(),
            Today,
            Today.AddDays(-1),
            null,
            [new CreateInvoiceItemRequest("Item 1", 1, 10m)]);

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInvoiceCommand.DueDate));
    }

    [Fact]
    public void CreateInvoiceValidator_WhenEmptyItems_ShouldFail()
    {
        var validator = new CreateInvoiceCommandValidator();
        var command = new CreateInvoiceCommand(
            Guid.NewGuid(),
            Today,
            Today.AddDays(10),
            null,
            []);

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInvoiceCommand.Items));
    }

    [Fact]
    public void UpdateInvoiceStatusValidator_WhenValidEnum_ShouldPass()
    {
        var validator = new UpdateInvoiceStatusCommandValidator();
        var command = new UpdateInvoiceStatusCommand(Guid.NewGuid(), InvoiceStatus.Sent);

        var result = validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
