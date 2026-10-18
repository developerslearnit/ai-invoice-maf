using InvoiceApp.Web.Models.Api;
using InvoiceApp.Web.Models.ViewModels;
using InvoiceApp.Web.Services.ApiClients;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceApp.Web.Controllers;

[Route("customers")]
public sealed class CustomersController : Controller
{
    private readonly ICustomerApiClient _customerApiClient;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(ICustomerApiClient customerApiClient, ILogger<CustomersController> logger)
    {
        _customerApiClient = customerApiClient;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index([FromQuery] string? search, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Customers";

        var response = await _customerApiClient.GetCustomersAsync(cancellationToken);
        if (!response.IsSuccess || response.Data is null)
        {
            return View(new CustomerListViewModel
            {
                ErrorMessage = response.ErrorMessage ?? "Unable to retrieve customers. Please ensure the backend API is active."
            });
        }

        var customers = response.Data.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            customers = customers.Where(c =>
                c.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                c.Email.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                (c.Phone != null && c.Phone.Contains(term, StringComparison.OrdinalIgnoreCase)));
        }

        var items = new List<CustomerItemViewModel>();
        foreach (var c in customers)
        {
            items.Add(new CustomerItemViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                Phone = c.Phone,
                Address = c.Address
            });
        }

        var viewModel = new CustomerListViewModel
        {
            Customers = items,
            SearchTerm = search
        };

        return View(viewModel);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Customer Details";

        var customerTask = _customerApiClient.GetCustomerByIdAsync(id, cancellationToken);
        var summaryTask = _customerApiClient.GetCustomerSummaryAsync(id, cancellationToken);
        var invoicesTask = _customerApiClient.GetCustomerInvoicesAsync(id, cancellationToken);

        await Task.WhenAll(customerTask, summaryTask, invoicesTask);

        var customerResult = await customerTask;
        var summaryResult = await summaryTask;
        var invoicesResult = await invoicesTask;

        if (!customerResult.IsSuccess || customerResult.Data is null)
        {
            return View(new CustomerDetailsViewModel
            {
                ErrorMessage = customerResult.ErrorMessage ?? "Customer not found."
            });
        }

        var viewModel = new CustomerDetailsViewModel
        {
            Customer = customerResult.Data,
            Summary = summaryResult.Data ?? new(id, customerResult.Data.Name, 0, 0, 0, 0, 0, 0),
            Invoices = invoicesResult.Data ?? []
        };

        return View(viewModel);
    }

    [HttpGet("get/{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var response = await _customerApiClient.GetCustomerByIdAsync(id, cancellationToken);
        if (!response.IsSuccess || response.Data is null)
        {
            return NotFound(new { message = response.ErrorMessage ?? "Customer not found" });
        }

        return Json(response.Data);
    }

    [HttpGet("list")]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var response = await _customerApiClient.GetCustomersAsync(cancellationToken);
        return Json(response.Data ?? []);
    }

    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromBody] CustomerFormInputModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );
            return BadRequest(new { success = false, message = "Please fix the validation errors.", errors });
        }

        var apiRequest = new CreateCustomerApiRequest(
            model.Name.Trim(),
            model.Email.Trim(),
            string.IsNullOrWhiteSpace(model.Phone) ? null : model.Phone.Trim(),
            string.IsNullOrWhiteSpace(model.Address) ? null : model.Address.Trim());

        var response = await _customerApiClient.CreateCustomerAsync(apiRequest, cancellationToken);
        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode, new
            {
                success = false,
                message = response.ErrorMessage ?? "Failed to create customer.",
                errors = response.ValidationErrors
            });
        }

        return Json(new { success = true, id = response.Data, message = "Customer created successfully." });
    }

    [HttpPost("edit/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, [FromBody] CustomerFormInputModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );
            return BadRequest(new { success = false, message = "Please fix the validation errors.", errors });
        }

        var apiRequest = new UpdateCustomerApiRequest(
            model.Name.Trim(),
            model.Email.Trim(),
            string.IsNullOrWhiteSpace(model.Phone) ? null : model.Phone.Trim(),
            string.IsNullOrWhiteSpace(model.Address) ? null : model.Address.Trim());

        var response = await _customerApiClient.UpdateCustomerAsync(id, apiRequest, cancellationToken);
        if (!response.IsSuccess)
        {
            return StatusCode(response.StatusCode, new
            {
                success = false,
                message = response.ErrorMessage ?? "Failed to update customer.",
                errors = response.ValidationErrors
            });
        }

        return Json(new { success = true, message = "Customer updated successfully." });
    }
}
