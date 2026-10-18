using System.Text.RegularExpressions;
using InvoiceApp.Domain.Common;
using InvoiceApp.Domain.Errors;

namespace InvoiceApp.Domain.Entities;

public sealed partial class Customer : AggregateRoot
{
    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex EmailRegex();

    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? Phone { get; private set; }
    public string? Address { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    private Customer() : base()
    {
    }

    private Customer(
        Guid id,
        string name,
        string email,
        string? phone,
        string? address,
        DateTime createdAtUtc) : base(id)
    {
        Name = name;
        Email = email;
        Phone = phone;
        Address = address;
        CreatedAtUtc = createdAtUtc;
    }

    public static Result<Customer> Create(
        string name,
        string email,
        string? phone = null,
        string? address = null,
        DateTime? createdAtUtc = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<Customer>(CustomerErrors.EmptyName);
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            return Result.Failure<Customer>(CustomerErrors.EmptyEmail);
        }

        var trimmedEmail = email.Trim();
        if (!EmailRegex().IsMatch(trimmedEmail))
        {
            return Result.Failure<Customer>(CustomerErrors.InvalidEmail);
        }

        var customer = new Customer(
            Guid.NewGuid(),
            name.Trim(),
            trimmedEmail.ToLowerInvariant(),
            phone?.Trim(),
            address?.Trim(),
            createdAtUtc ?? DateTime.UtcNow);

        return Result.Success(customer);
    }

    public Result Update(
        string name,
        string email,
        string? phone = null,
        string? address = null,
        DateTime? updatedAtUtc = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure(CustomerErrors.EmptyName);
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            return Result.Failure(CustomerErrors.EmptyEmail);
        }

        var trimmedEmail = email.Trim();
        if (!EmailRegex().IsMatch(trimmedEmail))
        {
            return Result.Failure(CustomerErrors.InvalidEmail);
        }

        Name = name.Trim();
        Email = trimmedEmail.ToLowerInvariant();
        Phone = phone?.Trim();
        Address = address?.Trim();
        UpdatedAtUtc = updatedAtUtc ?? DateTime.UtcNow;

        return Result.Success();
    }
}
