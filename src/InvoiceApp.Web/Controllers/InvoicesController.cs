using InvoiceApp.Web.Models.Api;
using InvoiceApp.Web.Models.ViewModels;
using InvoiceApp.Web.Services.ApiClients;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceApp.Web.Controllers;

[Route("invoices")]
public sealed class InvoicesController : Controller
{
    private readonly IInvoiceApiClient _invoiceApiClient;
    private readonly ICustomerApiClient _customerApiClient;
    private readonly ILogger<InvoicesController> _logger;

    public InvoicesController(
        IInvoiceApiClient invoiceApiClient,
        ICustomerApiClient customerApiClient,
        ILogger<InvoicesController> logger)
    {
        _invoiceApiClient = invoiceApiClient;
        _customerApiClient = customerApiClient;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index([FromQuery] InvoiceFilterParameters filters, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Invoices";

        var invoicesTask = _invoiceApiClient.GetInvoicesAsync(filters, cancellationToken);
        var customersTask = _customerApiClient.GetCustomersAsync(cancellationToken);

        await Task.WhenAll(invoicesTask, customersTask);

        var invoicesResult = await invoicesTask;
        var customersResult = await customersTask;

        var viewModel = new InvoiceListViewModel
        {
            PagedInvoices = invoicesResult.Data ?? new([], filters.Page, filters.PageSize, 0),
            Filters = filters,
            AvailableCustomers = customersResult.Data ?? [],
            ErrorMessage = invoicesResult.ErrorMessage
        };

        return View(viewModel);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Invoice Details";

        var response = await _invoiceApiClient.GetInvoiceByIdAsync(id, cancellationToken);
        if (!response.IsSuccess || response.Data is null)
        {
            return View(new InvoiceDetailsViewModel
            {
                Invoice = null,
                ErrorMessage = response.ErrorMessage ?? "Invoice could not be loaded."
            });
        }

        var viewModel = new InvoiceDetailsViewModel
        {
            Invoice = response.Data
        };

        return View(viewModel);
    }

    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromBody] InvoiceCreateInputModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );
            return BadRequest(new { success = false, message = "Please check required fields.", errors });
        }

        if (model.Items == null || model.Items.Count == 0)
        {
            return BadRequest(new { success = false, message = "Invoice must contain at least one line item." });
        }

        var items = model.Items.Select(i => new CreateInvoiceItemApiRequest(
            i.Description.Trim(),
            i.Quantity,
            i.UnitPrice
        )).ToList();

        var apiRequest = new CreateInvoiceApiRequest(
            model.CustomerId,
            model.IssueDate,
            model.DueDate,
            string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim(),
            items);

        var response = await _invoiceApiClient.CreateInvoiceAsync(apiRequest, cancellationToken);
        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode, new
            {
                success = false,
                message = response.ErrorMessage ?? "Failed to create invoice.",
                errors = response.ValidationErrors
            });
        }

        return Json(new { success = true, id = response.Data, message = "Invoice created successfully." });
    }

    [HttpPost("{id:guid}/status")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] InvoiceStatusUpdateInputModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { success = false, message = "Invalid status update payload." });
        }

        var response = await _invoiceApiClient.UpdateInvoiceStatusAsync(id, model.Status, cancellationToken);
        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode, new
            {
                success = false,
                message = response.ErrorMessage ?? "Failed to update invoice status."
            });
        }

        return Json(new { success = true, message = $"Invoice status updated to {model.Status}." });
    }

    [HttpPost("{id:guid}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var response = await _invoiceApiClient.DeleteInvoiceAsync(id, cancellationToken);
        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode, new
            {
                success = false,
                message = response.ErrorMessage ?? "Failed to delete invoice."
            });
        }

        return Json(new { success = true, message = "Invoice draft deleted successfully." });
    }
}
