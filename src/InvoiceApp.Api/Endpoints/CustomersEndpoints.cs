using FluentValidation;
using InvoiceApp.Api.Common;
using InvoiceApp.Application.Common.Messaging;
using InvoiceApp.Application.DTOs;
using InvoiceApp.Application.Features.Customers.Commands.CreateCustomer;
using InvoiceApp.Application.Features.Customers.Commands.UpdateCustomer;
using InvoiceApp.Application.Features.Customers.Queries.GetCustomerById;
using InvoiceApp.Application.Features.Customers.Queries.GetCustomerInvoices;
using InvoiceApp.Application.Features.Customers.Queries.GetCustomers;
using InvoiceApp.Application.Features.Customers.Queries.GetCustomerSummary;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceApp.Api.Endpoints;

public static class CustomersEndpoints
{
    public static IEndpointRouteBuilder MapCustomerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/customers")
            .WithTags("Customers");

        group.MapPost("/", CreateCustomerAsync)
            .WithName("CreateCustomer")
            .WithSummary("Creates a new customer")
            .Produces(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapGet("/", GetCustomersAsync)
            .WithName("GetCustomers")
            .WithSummary("Lists all registered customers")
            .Produces<IReadOnlyList<Domain.Models.CustomerDto>>(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", GetCustomerByIdAsync)
            .WithName("GetCustomerById")
            .WithSummary("Gets customer details by unique ID")
            .Produces<Domain.Models.CustomerDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPut("/{id:guid}", UpdateCustomerAsync)
            .WithName("UpdateCustomer")
            .WithSummary("Updates existing customer details")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapGet("/{customerId:guid}/invoices", GetCustomerInvoicesAsync)
            .WithName("GetCustomerInvoices")
            .WithSummary("Retrieves all invoices belonging to a specific customer")
            .Produces<IReadOnlyList<Domain.Models.InvoiceListItemDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/{customerId:guid}/summary", GetCustomerSummaryAsync)
            .WithName("GetCustomerSummary")
            .WithSummary("Retrieves the invoice balance summary for a specific customer (for AI Agent use)")
            .Produces<Domain.Models.CustomerSummaryDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<IResult> CreateCustomerAsync(
        [FromBody] CreateCustomerRequest request,
        [FromServices] IValidator<CreateCustomerCommand> validator,
        [FromServices] IDispatcher dispatcher,
        CancellationToken cancellationToken)
    {
        var command = new CreateCustomerCommand(request.Name, request.Email, request.Phone, request.Address);
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return CustomResults.ValidationProblem(validationResult);
        }

        var result = await dispatcher.SendAsync(command, cancellationToken);
        if (result.IsFailure)
        {
            return CustomResults.Problem(result.Error);
        }

        return Results.CreatedAtRoute("GetCustomerById", new { id = result.Value }, new { id = result.Value });
    }

    private static async Task<IResult> GetCustomersAsync(
        [FromServices] IDispatcher dispatcher,
        CancellationToken cancellationToken)
    {
        var result = await dispatcher.QueryAsync(new GetCustomersQuery(), cancellationToken);
        return Results.Ok(result.Value);
    }

    private static async Task<IResult> GetCustomerByIdAsync(
        [FromRoute] Guid id,
        [FromServices] IDispatcher dispatcher,
        CancellationToken cancellationToken)
    {
        var result = await dispatcher.QueryAsync(new GetCustomerByIdQuery(id), cancellationToken);
        if (result.IsFailure)
        {
            return CustomResults.Problem(result.Error);
        }

        return Results.Ok(result.Value);
    }

    private static async Task<IResult> UpdateCustomerAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateCustomerRequest request,
        [FromServices] IValidator<UpdateCustomerCommand> validator,
        [FromServices] IDispatcher dispatcher,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCustomerCommand(id, request.Name, request.Email, request.Phone, request.Address);
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return CustomResults.ValidationProblem(validationResult);
        }

        var result = await dispatcher.SendAsync(command, cancellationToken);
        if (result.IsFailure)
        {
            return CustomResults.Problem(result.Error);
        }

        return Results.NoContent();
    }

    private static async Task<IResult> GetCustomerInvoicesAsync(
        [FromRoute] Guid customerId,
        [FromServices] IDispatcher dispatcher,
        CancellationToken cancellationToken)
    {
        var result = await dispatcher.QueryAsync(new GetCustomerInvoicesQuery(customerId), cancellationToken);
        if (result.IsFailure)
        {
            return CustomResults.Problem(result.Error);
        }

        return Results.Ok(result.Value);
    }

    private static async Task<IResult> GetCustomerSummaryAsync(
        [FromRoute] Guid customerId,
        [FromServices] IDispatcher dispatcher,
        CancellationToken cancellationToken)
    {
        var result = await dispatcher.QueryAsync(new GetCustomerSummaryQuery(customerId), cancellationToken);
        if (result.IsFailure)
        {
            return CustomResults.Problem(result.Error);
        }

        return Results.Ok(result.Value);
    }
}
