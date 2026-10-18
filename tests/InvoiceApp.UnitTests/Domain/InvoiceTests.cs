using InvoiceApp.Domain.Entities;
using InvoiceApp.Domain.Enums;
using InvoiceApp.Domain.Errors;
using Xunit;

namespace InvoiceApp.UnitTests.Domain;

public sealed class InvoiceTests
{
    private static readonly DateOnly Today = new(2026, 10, 14);

    [Fact]
    public void Create_WithValidItems_ShouldCalculateTotalAmount()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var items = new List<(string Description, decimal Quantity, decimal UnitPrice)>
        {
            ("Item 1", 2, 100m),  // 200
            ("Item 2", 3, 50m)    // 150
        };

        // Act
        var result = Invoice.Create("INV-000001", customerId, Today, Today.AddDays(30), "Test Notes", items);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("INV-000001", result.Value.InvoiceNumber);
        Assert.Equal(InvoiceStatus.Draft, result.Value.Status);
        Assert.Equal(350m, result.Value.TotalAmount);
        Assert.Equal(2, result.Value.Items.Count);
        Assert.True(result.Value.CanBeDeleted);
    }

    [Fact]
    public void Create_WithDueDateBeforeIssueDate_ShouldFail()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var items = new List<(string Description, decimal Quantity, decimal UnitPrice)>
        {
            ("Item 1", 1, 100m)
        };

        // Act
        var result = Invoice.Create("INV-000001", customerId, Today, Today.AddDays(-1), null, items);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(InvoiceErrors.InvalidDueDate.Code, result.Error.Code);
    }

    [Fact]
    public void Create_WithEmptyItems_ShouldFail()
    {
        // Act
        var result = Invoice.Create("INV-000001", Guid.NewGuid(), Today, Today.AddDays(10), null, []);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(InvoiceErrors.EmptyItems.Code, result.Error.Code);
    }

    [Theory]
    // Allowed transitions
    [InlineData(InvoiceStatus.Draft, InvoiceStatus.Sent, true)]
    [InlineData(InvoiceStatus.Draft, InvoiceStatus.Cancelled, true)]
    [InlineData(InvoiceStatus.Sent, InvoiceStatus.Paid, true)]
    [InlineData(InvoiceStatus.Sent, InvoiceStatus.Overdue, true)]
    [InlineData(InvoiceStatus.Sent, InvoiceStatus.Cancelled, true)]
    [InlineData(InvoiceStatus.Overdue, InvoiceStatus.Paid, true)]
    [InlineData(InvoiceStatus.Overdue, InvoiceStatus.Cancelled, true)]
    // Same status
    [InlineData(InvoiceStatus.Draft, InvoiceStatus.Draft, true)]
    [InlineData(InvoiceStatus.Sent, InvoiceStatus.Sent, true)]
    // Disallowed transitions
    [InlineData(InvoiceStatus.Draft, InvoiceStatus.Paid, false)]
    [InlineData(InvoiceStatus.Draft, InvoiceStatus.Overdue, false)]
    [InlineData(InvoiceStatus.Sent, InvoiceStatus.Draft, false)]
    [InlineData(InvoiceStatus.Paid, InvoiceStatus.Draft, false)]
    [InlineData(InvoiceStatus.Paid, InvoiceStatus.Cancelled, false)]
    [InlineData(InvoiceStatus.Paid, InvoiceStatus.Sent, false)]
    [InlineData(InvoiceStatus.Cancelled, InvoiceStatus.Draft, false)]
    [InlineData(InvoiceStatus.Cancelled, InvoiceStatus.Paid, false)]
    [InlineData(InvoiceStatus.Overdue, InvoiceStatus.Draft, false)]
    public void TransitionTo_ShouldEnforceDomainStateRules(InvoiceStatus initialStatus, InvoiceStatus targetStatus, bool shouldSucceed)
    {
        // Arrange
        var invoice = Invoice.Create("INV-000001", Guid.NewGuid(), Today, Today.AddDays(10), null, [("Item", 1, 100m)]).Value;

        // Set up initial state via valid transitions
        if (initialStatus == InvoiceStatus.Sent)
        {
            invoice.TransitionTo(InvoiceStatus.Sent);
        }
        else if (initialStatus == InvoiceStatus.Paid)
        {
            invoice.TransitionTo(InvoiceStatus.Sent);
            invoice.TransitionTo(InvoiceStatus.Paid);
        }
        else if (initialStatus == InvoiceStatus.Overdue)
        {
            invoice.TransitionTo(InvoiceStatus.Sent);
            invoice.TransitionTo(InvoiceStatus.Overdue);
        }
        else if (initialStatus == InvoiceStatus.Cancelled)
        {
            invoice.TransitionTo(InvoiceStatus.Cancelled);
        }

        // Act
        var result = invoice.TransitionTo(targetStatus);

        // Assert
        if (shouldSucceed)
        {
            Assert.True(result.IsSuccess);
            Assert.Equal(targetStatus, invoice.Status);
        }
        else
        {
            Assert.True(result.IsFailure);
            Assert.Equal(InvoiceErrors.InvalidStatusTransition(initialStatus, targetStatus).Code, result.Error.Code);
        }
    }

    [Fact]
    public void CanBeDeleted_ShouldOnlyBeTrueForDraftStatus()
    {
        // Arrange
        var invoice = Invoice.Create("INV-000001", Guid.NewGuid(), Today, Today.AddDays(10), null, [("Item", 1, 100m)]).Value;
        Assert.True(invoice.CanBeDeleted);

        // Act & Assert for Sent
        invoice.TransitionTo(InvoiceStatus.Sent);
        Assert.False(invoice.CanBeDeleted);

        // Act & Assert for Paid
        invoice.TransitionTo(InvoiceStatus.Paid);
        Assert.False(invoice.CanBeDeleted);
    }

    [Fact]
    public void Update_WhenInvoiceIsNotDraft_ShouldFail()
    {
        // Arrange
        var invoice = Invoice.Create("INV-000001", Guid.NewGuid(), Today, Today.AddDays(10), null, [("Item", 1, 100m)]).Value;
        invoice.TransitionTo(InvoiceStatus.Sent);

        // Act
        var result = invoice.Update(Today, Today.AddDays(20), "Updated Notes", [("Item 2", 2, 200m)]);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(InvoiceErrors.CannotModifyNonDraft.Code, result.Error.Code);
    }
}
