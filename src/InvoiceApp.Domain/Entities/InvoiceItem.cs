using InvoiceApp.Domain.Common;
using InvoiceApp.Domain.Errors;

namespace InvoiceApp.Domain.Entities;

public sealed class InvoiceItem : Entity
{
    public Guid InvoiceId { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    public decimal LineTotal => Quantity * UnitPrice;

    private InvoiceItem() : base()
    {
    }

    private InvoiceItem(
        Guid id,
        Guid invoiceId,
        string description,
        decimal quantity,
        decimal unitPrice) : base(id)
    {
        InvoiceId = invoiceId;
        Description = description;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public static Result<InvoiceItem> Create(
        Guid invoiceId,
        string description,
        decimal quantity,
        decimal unitPrice)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return Result.Failure<InvoiceItem>(InvoiceErrors.EmptyItemDescription);
        }

        if (quantity <= 0)
        {
            return Result.Failure<InvoiceItem>(InvoiceErrors.InvalidItemQuantity);
        }

        if (unitPrice < 0)
        {
            return Result.Failure<InvoiceItem>(InvoiceErrors.InvalidItemUnitPrice);
        }

        var item = new InvoiceItem(
            Guid.NewGuid(),
            invoiceId,
            description.Trim(),
            quantity,
            unitPrice);

        return Result.Success(item);
    }
}
