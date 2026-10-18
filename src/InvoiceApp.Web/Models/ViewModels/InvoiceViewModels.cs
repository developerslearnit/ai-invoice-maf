using System.ComponentModel.DataAnnotations;
using InvoiceApp.Web.Models.Api;

namespace InvoiceApp.Web.Models.ViewModels;

public sealed class InvoiceListViewModel
{
    public PagedListApiModel<InvoiceListItemApiModel> PagedInvoices { get; init; } = new([], 1, 20, 0);
    public InvoiceFilterParameters Filters { get; init; } = new();
    public IReadOnlyList<CustomerApiModel> AvailableCustomers { get; init; } = [];
    public string? ErrorMessage { get; init; }
}

public sealed class InvoiceDetailsViewModel
{
    public InvoiceApiModel? Invoice { get; init; }
    public string? ErrorMessage { get; init; }

    public bool CanEdit => Invoice?.Status == InvoiceStatus.Draft;
    public bool CanDelete => Invoice?.Status == InvoiceStatus.Draft;
    public bool CanMarkSent => Invoice?.Status == InvoiceStatus.Draft;
    public bool CanMarkPaid => Invoice?.Status is InvoiceStatus.Sent or InvoiceStatus.Overdue;
    public bool CanMarkOverdue => Invoice?.Status == InvoiceStatus.Sent;
    public bool CanCancel => Invoice?.Status is InvoiceStatus.Draft or InvoiceStatus.Sent or InvoiceStatus.Overdue;
}

public sealed class InvoiceCreateInputModel
{
    [Required(ErrorMessage = "Please select a customer.")]
    public Guid CustomerId { get; set; }

    [Required(ErrorMessage = "Issue date is required.")]
    public DateOnly IssueDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    [Required(ErrorMessage = "Due date is required.")]
    public DateOnly DueDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(14));

    [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
    public string? Notes { get; set; }

    [MinLength(1, ErrorMessage = "At least one item is required on the invoice.")]
    public List<InvoiceItemInputModel> Items { get; set; } = [];
}

public sealed class InvoiceItemInputModel
{
    [Required(ErrorMessage = "Item description is required.")]
    [StringLength(300, ErrorMessage = "Description cannot exceed 300 characters.")]
    public string Description { get; set; } = string.Empty;

    [Range(0.0001, 1_000_000, ErrorMessage = "Quantity must be greater than 0.")]
    public decimal Quantity { get; set; } = 1;

    [Range(0.01, 100_000_000, ErrorMessage = "Unit price must be greater than 0.")]
    public decimal UnitPrice { get; set; } = 0;
}

public sealed class InvoiceStatusUpdateInputModel
{
    [Required]
    public Guid InvoiceId { get; set; }

    [Required]
    public InvoiceStatus Status { get; set; }
}
