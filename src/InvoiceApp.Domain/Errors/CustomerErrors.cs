using InvoiceApp.Domain.Common;

namespace InvoiceApp.Domain.Errors;

public static class CustomerErrors
{
    public static readonly Error EmptyName =
        Error.Validation("Customer.EmptyName", "Customer name cannot be empty.");

    public static readonly Error EmptyEmail =
        Error.Validation("Customer.EmptyEmail", "Customer email cannot be empty.");

    public static readonly Error InvalidEmail =
        Error.Validation("Customer.InvalidEmail", "Customer email format is invalid.");

    public static Error EmailNotUnique(string email) =>
        Error.Conflict("Customer.EmailNotUnique", $"The email '{email}' is already registered.");

    public static Error NotFound(Guid id) =>
        Error.NotFound("Customer.NotFound", $"Customer with ID '{id}' was not found.");
}
