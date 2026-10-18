using System.ComponentModel;
using System.Text.Json;
using InvoiceApp.Application.Common.Messaging;
using InvoiceApp.Application.Features.Customers.Commands.CreateCustomer;
using InvoiceApp.Application.Features.Customers.Commands.UpdateCustomer;
using InvoiceApp.Application.Features.Customers.Queries.GetCustomerById;
using InvoiceApp.Application.Features.Customers.Queries.GetCustomers;
using InvoiceApp.Application.Features.Customers.Queries.GetCustomerSummary;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace InvoiceApp.AI.Tools;

/// <summary>
/// Exposes customer management domain operations as autonomous AI tools (<see cref="AITool"/>)
/// callable by agents using Microsoft Agent Framework and Microsoft.Extensions.AI.
/// </summary>
/// <remarks>
/// <para>
/// Every tool delegates to the application layer via <see cref="IDispatcher"/>, maintaining strict
/// CQRS separation, FluentValidation enforcement, and EF Core / Dapper persistence.
/// </para>
/// <para>
/// Tool functions use <see cref="DescriptionAttribute"/> on methods and parameters so that LLM function
/// calling can inspect JSON schemas and dynamically extract argument types.
/// </para>
/// </remarks>
/// <param name="dispatcher">The in-process mediator/dispatcher for executing CQRS commands and queries.</param>
/// <param name="logger">Structured logger instance.</param>
public sealed class CustomerTools(
    IDispatcher dispatcher,
    ILogger<CustomerTools> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    /// <summary>
    /// Generates the list of <see cref="AITool"/> descriptors registered with the LLM via <see cref="AIFunctionFactory.Create"/>.
    /// </summary>
    /// <returns>An enumerable of AI function definitions for customer operations.</returns>
    public IEnumerable<AITool> GetTools()
    {
        yield return AIFunctionFactory.Create(
            CreateCustomer,
            nameof(CreateCustomer),
            "Creates a new customer record with name, email, and optional phone/address. Email must be unique.");

        yield return AIFunctionFactory.Create(
            UpdateCustomer,
            nameof(UpdateCustomer),
            "Updates an existing customer's contact details, phone, or address.");

        yield return AIFunctionFactory.Create(
            GetCustomerProfile,
            nameof(GetCustomerProfile),
            "Retrieves full customer summary including lifetime invoiced amount, total paid, and outstanding balance by customer ID.");

        yield return AIFunctionFactory.Create(
            FindCustomer,
            nameof(FindCustomer),
            "Finds a customer by matching name or email address.");

        yield return AIFunctionFactory.Create(
            ListCustomers,
            nameof(ListCustomers),
            "Lists all registered customers in the system.");
    }

    /// <summary>
    /// Creates a new customer profile in the system.
    /// </summary>
    /// <param name="name">Full name or business trade name of the customer.</param>
    /// <param name="email">Unique email address used for invoice delivery and notifications.</param>
    /// <param name="phone">Optional telephone or mobile number.</param>
    /// <param name="address">Optional registered billing address.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A JSON string containing the created customer GUID ID or validation error details.</returns>
    [Description("Creates a new customer. Requires full name and unique email address.")]
    public async Task<string> CreateCustomer(
        [Description("Full name of the customer or business")] string name,
        [Description("Unique email address for correspondence")] string email,
        [Description("Optional telephone number")] string? phone = null,
        [Description("Optional physical or registered billing address")] string? address = null,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Tool invoked: CreateCustomer for {Name} ({Email})", name, email);

        var command = new CreateCustomerCommand(name.Trim(), email.Trim(), phone?.Trim(), address?.Trim());
        var result = await dispatcher.SendAsync(command, cancellationToken);

        if (result.IsFailure)
        {
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = result.Error.Description
            }, JsonOptions);
        }

        return JsonSerializer.Serialize(new
        {
            success = true,
            customerId = result.Value,
            name = name.Trim(),
            email = email.Trim(),
            message = $"Customer '{name.Trim()}' created successfully with ID {result.Value}."
        }, JsonOptions);
    }

    /// <summary>
    /// Updates contact and profile details of an existing customer record.
    /// </summary>
    /// <param name="customerId">The GUID string identifier of the customer to update.</param>
    /// <param name="name">The updated customer name.</param>
    /// <param name="email">The updated customer email address.</param>
    /// <param name="phone">Optional updated phone number.</param>
    /// <param name="address">Optional updated billing address.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A JSON string indicating success or failure.</returns>
    [Description("Updates an existing customer's information.")]
    public async Task<string> UpdateCustomer(
        [Description("The GUID identifier of the customer")] string customerId,
        [Description("Updated full name")] string name,
        [Description("Updated email address")] string email,
        [Description("Updated telephone number")] string? phone = null,
        [Description("Updated billing address")] string? address = null,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Tool invoked: UpdateCustomer for {CustomerId}", customerId);

        if (!Guid.TryParse(customerId, out var id))
        {
            return JsonSerializer.Serialize(new { success = false, error = "Invalid customer ID format." }, JsonOptions);
        }

        var command = new UpdateCustomerCommand(id, name.Trim(), email.Trim(), phone?.Trim(), address?.Trim());
        var result = await dispatcher.SendAsync(command, cancellationToken);

        if (result.IsFailure)
        {
            return JsonSerializer.Serialize(new { success = false, error = result.Error.Description }, JsonOptions);
        }

        return JsonSerializer.Serialize(new
        {
            success = true,
            customerId = id,
            message = $"Customer '{name.Trim()}' updated successfully."
        }, JsonOptions);
    }

    /// <summary>
    /// Retrieves a comprehensive financial ledger summary for a specific customer, including total invoiced,
    /// payments settled, and outstanding debt balances.
    /// </summary>
    /// <param name="customerId">The GUID string identifier of the customer.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A JSON string representing the customer's financial profile.</returns>
    [Description("Gets customer financial summary, total invoiced, paid, and outstanding balance.")]
    public async Task<string> GetCustomerProfile(
        [Description("The GUID identifier of the customer")] string customerId,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Tool invoked: GetCustomerProfile for {CustomerId}", customerId);

        if (!Guid.TryParse(customerId, out var id))
        {
            return JsonSerializer.Serialize(new { success = false, error = "Invalid customer ID format." }, JsonOptions);
        }

        var result = await dispatcher.QueryAsync(new GetCustomerSummaryQuery(id), cancellationToken);
        if (result.IsFailure || result.Value is null)
        {
            return JsonSerializer.Serialize(new { success = false, error = result.Error.Description }, JsonOptions);
        }

        var summary = result.Value;
        return JsonSerializer.Serialize(new
        {
            success = true,
            customerId = summary.CustomerId,
            customerName = summary.CustomerName,
            totalInvoices = summary.TotalInvoices,
            totalInvoiced = summary.TotalInvoiced,
            totalPaid = summary.TotalPaid,
            totalOutstanding = summary.TotalOutstanding,
            paidInvoices = summary.PaidInvoices,
            outstandingInvoices = summary.OutstandingInvoices
        }, JsonOptions);
    }

    /// <summary>
    /// Searches the customer registry for records matching a substring query against name or email.
    /// </summary>
    /// <param name="searchTerm">Partial or full text to match against name or email.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A JSON string containing up to 5 matching customer records.</returns>
    [Description("Searches for a customer by partial name or email.")]
    public async Task<string> FindCustomer(
        [Description("Search term matching name or email")] string searchTerm,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Tool invoked: FindCustomer with term '{SearchTerm}'", searchTerm);

        var result = await dispatcher.QueryAsync(new GetCustomersQuery(), cancellationToken);
        if (result.IsFailure || result.Value is null)
        {
            return JsonSerializer.Serialize(new { success = false, error = "Unable to retrieve customers." }, JsonOptions);
        }

        var matched = result.Value
            .Where(c => c.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        c.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            .Take(5)
            .ToList();

        return JsonSerializer.Serialize(matched, JsonOptions);
    }

    /// <summary>
    /// Lists all registered customers within the system.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A JSON string containing the full collection of customers.</returns>
    [Description("Lists all registered customers in the system.")]
    public async Task<string> ListCustomers(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Tool invoked: ListCustomers");

        var result = await dispatcher.QueryAsync(new GetCustomersQuery(), cancellationToken);
        if (result.IsFailure || result.Value is null)
        {
            return JsonSerializer.Serialize(new { success = false, error = "Unable to retrieve customers." }, JsonOptions);
        }

        return JsonSerializer.Serialize(result.Value, JsonOptions);
    }
}
