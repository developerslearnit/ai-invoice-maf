using FluentValidation;
using InvoiceApp.Api.Common;
using InvoiceApp.Application.Common.Messaging;
using InvoiceApp.Application.DTOs;
using InvoiceApp.Application.Features.Invoices.Commands.CreateInvoice;
using InvoiceApp.Application.Features.Invoices.Commands.DeleteInvoice;
using InvoiceApp.Application.Features.Invoices.Commands.UpdateInvoice;
using InvoiceApp.Application.Features.Invoices.Commands.UpdateInvoiceStatus;
using InvoiceApp.Application.Features.Invoices.Queries.GetInvoiceById;
using InvoiceApp.Application.Features.Invoices.Queries.GetInvoiceByNumber;
using InvoiceApp.Application.Features.Invoices.Queries.GetInvoices;
using InvoiceApp.Application.Features.Invoices.Queries.GetInvoiceSummary;
using InvoiceApp.Application.Features.Invoices.Queries.GetOverdueInvoices;
using InvoiceApp.Domain.Enums;
using InvoiceApp.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceApp.Api.Endpoints;

public static class InvoicesEndpoints
{
    public static IEndpointRouteBuilder MapInvoiceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/invoices")
            .WithTags("Invoices");

        group.MapPost("/", CreateInvoiceAsync)
            .WithName("CreateInvoice")
            .WithSummary("Creates a new draft invoice")
            .Produces(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        group.MapGet("/", GetInvoicesAsync)
            .WithName("GetInvoices")
            .WithSummary("Retrieves a filtered, paginated list of invoices")
            .Produces<PagedList<InvoiceListItemDto>>(StatusCodes.Status200OK);

        group.MapGet("/overdue", GetOverdueInvoicesAsync)
            .WithName("GetOverdueInvoices")
            .WithSummary("Retrieves all invoices past their due date that are not paid or cancelled (for AI Agent use)")
            .Produces<IReadOnlyList<InvoiceListItemDto>>(StatusCodes.Status200OK);

        group.MapGet("/summary", GetInvoiceSummaryAsync)
            .WithName("GetInvoiceSummary")
            .WithSummary("Retrieves aggregate business metrics for invoices (for AI Agent use)")
            .Produces<InvoiceSummaryDto>(StatusCodes.Status200OK);

        group.MapGet("/by-number/{invoiceNumber}", GetInvoiceByNumberAsync)
            .WithName("GetInvoiceByNumber")
            .WithSummary("Gets invoice details by formatted invoice number (e.g., INV-000001)")
            .Produces<InvoiceDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/{id:guid}", GetInvoiceByIdAsync)
            .WithName("GetInvoiceById")
            .WithSummary("Gets invoice details by unique ID")
            .Produces<InvoiceDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPut("/{id:guid}", UpdateInvoiceAsync)
            .WithName("UpdateInvoice")
            .WithSummary("Updates a draft invoice")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        group.MapDelete("/{id:guid}", DeleteInvoiceAsync)
            .WithName("DeleteInvoice")
            .WithSummary("Deletes an invoice (only permitted if in Draft status)")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        group.MapPatch("/{id:guid}/status", UpdateInvoiceStatusAsync)
            .WithName("UpdateInvoiceStatus")
            .WithSummary("Updates the status of an invoice following domain transition rules")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        group.MapPost("/seed", async (
            [FromServices] InvoiceApp.Infrastructure.Persistence.DatabaseInitializer initializer,
            CancellationToken cancellationToken) =>
        {
            await initializer.InitializeAsync(forceReseed: true, cancellationToken);
            return Results.Ok(new { message = "Database seeded with 10 customers and 50 invoices (5 each)." });
        })
        .WithName("SeedSampleData")
        .WithSummary("Generates 10 customers and 5 invoices for each in SQL Server")
        .Produces(StatusCodes.Status200OK);

        return app;
    }

    private static async Task<IResult> CreateInvoiceAsync(
        [FromBody] CreateInvoiceRequest request,
        [FromServices] IValidator<CreateInvoiceCommand> validator,
        [FromServices] IDispatcher dispatcher,
        CancellationToken cancellationToken)
    {
        var command = new CreateInvoiceCommand(
            request.CustomerId,
            request.IssueDate,
            request.DueDate,
            request.Notes,
            request.Items);

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

        return Results.CreatedAtRoute("GetInvoiceById", new { id = result.Value }, new { id = result.Value });
    }

    private static async Task<IResult> GetInvoicesAsync(
        [FromQuery] InvoiceStatus? status,
        [FromQuery] Guid? customerId,
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate,
        [FromQuery] decimal? minimumAmount,
        [FromQuery] decimal? maximumAmount,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromServices] IDispatcher dispatcher = null!,
        CancellationToken cancellationToken = default)
    {
        var parameters = new InvoiceSearchParameters(
            status,
            customerId,
            fromDate,
            toDate,
            minimumAmount,
            maximumAmount,
            search,
            page,
            pageSize);

        var result = await dispatcher.QueryAsync(new GetInvoicesQuery(parameters), cancellationToken);
        return Results.Ok(result.Value);
    }

    private static async Task<IResult> GetInvoiceByIdAsync(
        [FromRoute] Guid id,
        [FromServices] IDispatcher dispatcher,
        CancellationToken cancellationToken)
    {
        var result = await dispatcher.QueryAsync(new GetInvoiceByIdQuery(id), cancellationToken);
        if (result.IsFailure)
        {
            return CustomResults.Problem(result.Error);
        }

        return Results.Ok(result.Value);
    }

    private static async Task<IResult> GetInvoiceByNumberAsync(
        [FromRoute] string invoiceNumber,
        [FromServices] IDispatcher dispatcher,
        CancellationToken cancellationToken)
    {
        var result = await dispatcher.QueryAsync(new GetInvoiceByNumberQuery(invoiceNumber), cancellationToken);
        if (result.IsFailure)
        {
            return CustomResults.Problem(result.Error);
        }

        return Results.Ok(result.Value);
    }

    private static async Task<IResult> UpdateInvoiceAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateInvoiceRequest request,
        [FromServices] IValidator<UpdateInvoiceCommand> validator,
        [FromServices] IDispatcher dispatcher,
        CancellationToken cancellationToken)
    {
        var command = new UpdateInvoiceCommand(
            id,
            request.IssueDate,
            request.DueDate,
            request.Notes,
            request.Items);

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

    private static async Task<IResult> DeleteInvoiceAsync(
        [FromRoute] Guid id,
        [FromServices] IDispatcher dispatcher,
        CancellationToken cancellationToken)
    {
        var result = await dispatcher.SendAsync(new DeleteInvoiceCommand(id), cancellationToken);
        if (result.IsFailure)
        {
            return CustomResults.Problem(result.Error);
        }

        return Results.NoContent();
    }

    private static async Task<IResult> UpdateInvoiceStatusAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateInvoiceStatusRequest request,
        [FromServices] IValidator<UpdateInvoiceStatusCommand> validator,
        [FromServices] IDispatcher dispatcher,
        CancellationToken cancellationToken)
    {
        var command = new UpdateInvoiceStatusCommand(id, request.Status);
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

    private static async Task<IResult> GetOverdueInvoicesAsync(
        [FromServices] IDispatcher dispatcher,
        CancellationToken cancellationToken)
    {
        var result = await dispatcher.QueryAsync(new GetOverdueInvoicesQuery(), cancellationToken);
        return Results.Ok(result.Value);
    }

    private static async Task<IResult> GetInvoiceSummaryAsync(
        [FromServices] IDispatcher dispatcher,
        CancellationToken cancellationToken)
    {
        var result = await dispatcher.QueryAsync(new GetInvoiceSummaryQuery(), cancellationToken);
        return Results.Ok(result.Value);
    }
}
