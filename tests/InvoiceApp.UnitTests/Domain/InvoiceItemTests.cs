using InvoiceApp.Domain.Entities;
using InvoiceApp.Domain.Errors;
using Xunit;

namespace InvoiceApp.UnitTests.Domain;

public sealed class InvoiceItemTests
{
    [Fact]
    public void Create_WithValidInputs_ShouldCalculateLineTotalCorrectly()
    {
        // Act
        var result = InvoiceItem.Create(Guid.NewGuid(), "Consulting hours", 5m, 150m);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Consulting hours", result.Value.Description);
        Assert.Equal(5m, result.Value.Quantity);
        Assert.Equal(150m, result.Value.UnitPrice);
        Assert.Equal(750m, result.Value.LineTotal);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10.5)]
    public void Create_WithZeroOrNegativeQuantity_ShouldFail(decimal quantity)
    {
        // Act
        var result = InvoiceItem.Create(Guid.NewGuid(), "Widget", quantity, 100m);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(InvoiceErrors.InvalidItemQuantity.Code, result.Error.Code);
    }

    [Fact]
    public void Create_WithNegativeUnitPrice_ShouldFail()
    {
        // Act
        var result = InvoiceItem.Create(Guid.NewGuid(), "Widget", 2m, -5m);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(InvoiceErrors.InvalidItemUnitPrice.Code, result.Error.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithEmptyDescription_ShouldFail(string? description)
    {
        // Act
        var result = InvoiceItem.Create(Guid.NewGuid(), description!, 2m, 10m);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(InvoiceErrors.EmptyItemDescription.Code, result.Error.Code);
    }
}
