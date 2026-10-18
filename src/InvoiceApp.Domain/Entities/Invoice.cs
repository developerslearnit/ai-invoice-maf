using InvoiceApp.Domain.Common;
using InvoiceApp.Domain.Enums;
using InvoiceApp.Domain.Errors;

namespace InvoiceApp.Domain.Entities;

public sealed class Invoice : AggregateRoot
{
    private readonly List<InvoiceItem> _items = [];

    public string InvoiceNumber { get; private set; } = string.Empty;
    public Guid CustomerId { get; private set; }
    public Customer? Customer { get; private set; }
    public DateOnly IssueDate { get; private set; }
    public DateOnly DueDate { get; private set; }
    public InvoiceStatus Status { get; private set; } = InvoiceStatus.Draft;
    public string? Notes { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    public IReadOnlyCollection<InvoiceItem> Items => _items.AsReadOnly();

    public decimal TotalAmount => _items.Sum(item => item.LineTotal);

    public bool CanBeDeleted => Status == InvoiceStatus.Draft;

    private Invoice() : base()
    {
    }

    private Invoice(
        Guid id,
        string invoiceNumber,
        Guid customerId,
        DateOnly issueDate,
        DateOnly dueDate,
        string? notes,
        DateTime createdAtUtc) : base(id)
    {
        InvoiceNumber = invoiceNumber;
        CustomerId = customerId;
        IssueDate = issueDate;
        DueDate = dueDate;
        Notes = notes;
        Status = InvoiceStatus.Draft;
        CreatedAtUtc = createdAtUtc;
    }

    public static Result<Invoice> Create(
        string invoiceNumber,
        Guid customerId,
        DateOnly issueDate,
        DateOnly dueDate,
        string? notes,
        IEnumerable<(string Description, decimal Quantity, decimal UnitPrice)> items,
        DateTime? createdAtUtc = null)
    {
        if (string.IsNullOrWhiteSpace(invoiceNumber))
        {
            return Result.Failure<Invoice>(InvoiceErrors.EmptyInvoiceNumber);
        }

        if (customerId == Guid.Empty)
        {
            return Result.Failure<Invoice>(InvoiceErrors.EmptyCustomerId);
        }

        if (dueDate < issueDate)
        {
            return Result.Failure<Invoice>(InvoiceErrors.InvalidDueDate);
        }

        var itemsList = items?.ToList() ?? [];
        if (itemsList.Count == 0)
        {
            return Result.Failure<Invoice>(InvoiceErrors.EmptyItems);
        }

        var invoice = new Invoice(
            Guid.NewGuid(),
            invoiceNumber.Trim(),
            customerId,
            issueDate,
            dueDate,
            notes?.Trim(),
            createdAtUtc ?? DateTime.UtcNow);

        foreach (var (description, quantity, unitPrice) in itemsList)
        {
            var itemResult = InvoiceItem.Create(invoice.Id, description, quantity, unitPrice);
            if (itemResult.IsFailure)
            {
                return Result.Failure<Invoice>(itemResult.Error);
            }

            invoice._items.Add(itemResult.Value);
        }

        return Result.Success(invoice);
    }

    public Result Update(
        DateOnly issueDate,
        DateOnly dueDate,
        string? notes,
        IEnumerable<(string Description, decimal Quantity, decimal UnitPrice)> items,
        DateTime? updatedAtUtc = null)
    {
        if (Status != InvoiceStatus.Draft)
        {
            return Result.Failure(InvoiceErrors.CannotModifyNonDraft);
        }

        if (dueDate < issueDate)
        {
            return Result.Failure(InvoiceErrors.InvalidDueDate);
        }

        var itemsList = items?.ToList() ?? [];
        if (itemsList.Count == 0)
        {
            return Result.Failure(InvoiceErrors.EmptyItems);
        }

        var newItems = new List<InvoiceItem>();
        foreach (var (description, quantity, unitPrice) in itemsList)
        {
            var itemResult = InvoiceItem.Create(Id, description, quantity, unitPrice);
            if (itemResult.IsFailure)
            {
                return Result.Failure(itemResult.Error);
            }

            newItems.Add(itemResult.Value);
        }

        IssueDate = issueDate;
        DueDate = dueDate;
        Notes = notes?.Trim();
        _items.Clear();
        _items.AddRange(newItems);
        UpdatedAtUtc = updatedAtUtc ?? DateTime.UtcNow;

        return Result.Success();
    }

    public Result TransitionTo(InvoiceStatus targetStatus, DateTime? updatedAtUtc = null)
    {
        if (Status == targetStatus)
        {
            return Result.Success();
        }

        var isValidTransition = (Status, targetStatus) switch
        {
            (InvoiceStatus.Draft, InvoiceStatus.Sent) => true,
            (InvoiceStatus.Draft, InvoiceStatus.Cancelled) => true,
            (InvoiceStatus.Sent, InvoiceStatus.Paid) => true,
            (InvoiceStatus.Sent, InvoiceStatus.Overdue) => true,
            (InvoiceStatus.Sent, InvoiceStatus.Cancelled) => true,
            (InvoiceStatus.Overdue, InvoiceStatus.Paid) => true,
            (InvoiceStatus.Overdue, InvoiceStatus.Cancelled) => true,
            _ => false
        };

        if (!isValidTransition)
        {
            return Result.Failure(InvoiceErrors.InvalidStatusTransition(Status, targetStatus));
        }

        Status = targetStatus;
        UpdatedAtUtc = updatedAtUtc ?? DateTime.UtcNow;

        return Result.Success();
    }
}
