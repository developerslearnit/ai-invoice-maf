using System.ComponentModel.DataAnnotations;
using InvoiceApp.Web.Models.Api;

namespace InvoiceApp.Web.Models.ViewModels;

public sealed class CustomerListViewModel
{
    public IReadOnlyList<CustomerItemViewModel> Customers { get; init; } = [];
    public string? SearchTerm { get; init; }
    public string? ErrorMessage { get; init; }
}

public sealed class CustomerItemViewModel
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string? Address { get; init; }
    public int TotalInvoices { get; init; }
    public decimal TotalOutstanding { get; init; }
}

public sealed class CustomerDetailsViewModel
{
    public CustomerApiModel? Customer { get; init; }
    public CustomerSummaryApiModel? Summary { get; init; }
    public IReadOnlyList<InvoiceListItemApiModel> Invoices { get; init; } = [];
    public string? ErrorMessage { get; init; }
}

public sealed class CustomerFormInputModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Customer name is required.")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    [StringLength(320, ErrorMessage = "Email cannot exceed 320 characters.")]
    public string Email { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "Phone number cannot exceed 50 characters.")]
    public string? Phone { get; set; }

    [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters.")]
    public string? Address { get; set; }
}
