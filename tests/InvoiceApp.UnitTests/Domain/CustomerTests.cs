using InvoiceApp.Domain.Entities;
using InvoiceApp.Domain.Errors;
using Xunit;

namespace InvoiceApp.UnitTests.Domain;

public sealed class CustomerTests
{
    [Fact]
    public void Create_WithValidInputs_ShouldSucceed()
    {
        // Act
        var result = Customer.Create("Contoso Ltd", "contact@contoso.com", "+1-555-1234", "123 Main St");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
        Assert.Equal("Contoso Ltd", result.Value.Name);
        Assert.Equal("contact@contoso.com", result.Value.Email);
        Assert.Equal("+1-555-1234", result.Value.Phone);
        Assert.Equal("123 Main St", result.Value.Address);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithEmptyName_ShouldReturnEmptyNameError(string? name)
    {
        // Act
        var result = Customer.Create(name!, "contact@contoso.com");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(CustomerErrors.EmptyName.Code, result.Error.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithEmptyEmail_ShouldReturnEmptyEmailError(string? email)
    {
        // Act
        var result = Customer.Create("Contoso Ltd", email!);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(CustomerErrors.EmptyEmail.Code, result.Error.Code);
    }

    [Theory]
    [InlineData("plainaddress")]
    [InlineData("missingatsign.com")]
    [InlineData("@missingusername.com")]
    public void Create_WithInvalidEmailFormat_ShouldReturnInvalidEmailError(string email)
    {
        // Act
        var result = Customer.Create("Contoso Ltd", email);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(CustomerErrors.InvalidEmail.Code, result.Error.Code);
    }

    [Fact]
    public void Update_WithValidInputs_ShouldUpdateProperties()
    {
        // Arrange
        var customer = Customer.Create("Old Name", "old@contoso.com").Value;

        // Act
        var result = customer.Update("New Name", "new@contoso.com", "+1-555-9999", "456 Oak St");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("New Name", customer.Name);
        Assert.Equal("new@contoso.com", customer.Email);
        Assert.Equal("+1-555-9999", customer.Phone);
        Assert.Equal("456 Oak St", customer.Address);
        Assert.NotNull(customer.UpdatedAtUtc);
    }
}
