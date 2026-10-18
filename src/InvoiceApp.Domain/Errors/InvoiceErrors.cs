using InvoiceApp.Domain.Common;
using InvoiceApp.Domain.Enums;

namespace InvoiceApp.Domain.Errors;

public static class InvoiceErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Invoice.NotFound", $"Invoice with ID '{id}' was not found.");

    public static Error NotFoundByNumber(string invoiceNumber) =>
        Error.NotFound("Invoice.NotFoundByNumber", $"Invoice with number '{invoiceNumber}' was not found.");

    public static readonly Error EmptyCustomerId =
        Error.Validation("Invoice.EmptyCustomerId", "CustomerId is required.");

    public static readonly Error EmptyInvoiceNumber =
        Error.Validation("Invoice.EmptyInvoiceNumber", "Invoice number is required.");

    public static readonly Error EmptyItems =
        Error.Validation("Invoice.EmptyItems", "An invoice must contain at least one item.");

    public static readonly Error InvalidDueDate =
        Error.Validation("Invoice.InvalidDueDate", "Due date cannot be earlier than issue date.");

    public static readonly Error EmptyItemDescription =
        Error.Validation("InvoiceItem.EmptyDescription", "Invoice item description cannot be empty.");

    public static readonly Error InvalidItemQuantity =
        Error.Validation("InvoiceItem.InvalidQuantity", "Quantity must be greater than zero.");

    public static readonly Error InvalidItemUnitPrice =
        Error.Validation("InvoiceItem.InvalidUnitPrice", "Unit price must be greater than or equal to zero.");

    public static Error InvalidStatusTransition(InvoiceStatus currentStatus, InvoiceStatus newStatus) =>
        Error.Failure("Invoice.InvalidStatusTransition", $"Cannot transition invoice status from '{currentStatus}' to '{newStatus}'.");

    public static readonly Error CannotDeleteNonDraft =
        Error.Failure("Invoice.CannotDeleteNonDraft", "Only invoices in Draft status can be deleted.");

    public static readonly Error CannotModifyNonDraft =
        Error.Failure("Invoice.CannotModifyNonDraft", "Only invoices in Draft status can be modified.");
}
