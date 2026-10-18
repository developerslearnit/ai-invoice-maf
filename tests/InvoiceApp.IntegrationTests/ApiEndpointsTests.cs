using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using InvoiceApp.Application.DTOs;
using InvoiceApp.Domain.Entities;
using InvoiceApp.Domain.Enums;
using InvoiceApp.Domain.Models;
using InvoiceApp.Domain.Repositories;
using InvoiceApp.Domain.Services;
using InvoiceApp.Domain.UnitOfWork;
using InvoiceApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace InvoiceApp.IntegrationTests;

public sealed class ApiEndpointsTests : IClassFixture<InvoiceAppTestFactory>
{
    private readonly HttpClient _client;
    private readonly InvoiceAppTestFactory _factory;

    public ApiEndpointsTests(InvoiceAppTestFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostCustomer_WithInvalidEmail_ShouldReturn400ValidationProblem()
    {
        // Arrange
        var request = new CreateCustomerRequest("Acme Corp", "not-an-email");

        // Act
        var response = await _client.PostAsJsonAsync("/api/customers", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problem);
        Assert.Contains("email", problem.Errors.Keys, StringComparer.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task PostInvoice_WithDueDateBeforeIssueDate_ShouldReturn400ValidationProblem()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var request = new CreateInvoiceRequest(
            Guid.NewGuid(),
            today,
            today.AddDays(-2),
            "Notes",
            [new CreateInvoiceItemRequest("Item 1", 1, 100m)]);

        // Act
        var response = await _client.PostAsJsonAsync("/api/invoices", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problem);
        Assert.Contains("dueDate", problem.Errors.Keys, StringComparer.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetCustomerById_WhenNotFound_ShouldReturn404ProblemDetails()
    {
        // Act
        var response = await _client.GetAsync($"/api/customers/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problem);
        Assert.Equal(404, problem.Status);
    }

    [Fact]
    public async Task GetInvoicesSummary_ShouldReturn200WithSummaryMetrics()
    {
        // Act
        var response = await _client.GetAsync("/api/invoices/summary");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var summary = await response.Content.ReadFromJsonAsync<InvoiceSummaryDto>();
        Assert.NotNull(summary);
        Assert.True(summary.TotalInvoices >= 0);
    }

    [Fact]
    public async Task GetOverdueInvoices_ShouldReturn200()
    {
        // Act
        var response = await _client.GetAsync("/api/invoices/overdue");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var items = await response.Content.ReadFromJsonAsync<List<InvoiceListItemDto>>();
        Assert.NotNull(items);
    }

    [Fact]
    public async Task GetInvoices_WithFiltering_ShouldReturnPagedList()
    {
        // Act
        var response = await _client.GetAsync("/api/invoices?page=1&pageSize=10");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var paged = await response.Content.ReadFromJsonAsync<PagedList<InvoiceListItemDto>>();
        Assert.NotNull(paged);
        Assert.Equal(1, paged.Page);
        Assert.Equal(10, paged.PageSize);
    }
}

public sealed class InvoiceAppTestFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Replace repositories with test doubles for integration testing
            services.RemoveAll<ICustomerRepository>();
            services.RemoveAll<ICustomerReadRepository>();
            services.RemoveAll<IInvoiceRepository>();
            services.RemoveAll<IInvoiceReadRepository>();
            services.RemoveAll<IInvoiceNumberGenerator>();
            services.RemoveAll<IUnitOfWork>();

            services.AddSingleton<ICustomerRepository, TestCustomerRepository>();
            services.AddSingleton<ICustomerReadRepository, TestCustomerReadRepository>();
            services.AddSingleton<IInvoiceRepository, TestInvoiceRepository>();
            services.AddSingleton<IInvoiceReadRepository, TestInvoiceReadRepository>();
            services.AddSingleton<IInvoiceNumberGenerator, TestInvoiceNumberGenerator>();
            services.AddSingleton<IUnitOfWork, TestUnitOfWork>();
        });
    }
}

internal sealed class TestCustomerRepository : ICustomerRepository
{
    private readonly List<Customer> _customers = [];
    public Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_customers.FirstOrDefault(c => c.Id == id));
    public Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        Task.FromResult(_customers.FirstOrDefault(c => c.Email == email.ToLowerInvariant()));
    public Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        _customers.Add(customer);
        return Task.CompletedTask;
    }
    public void Update(Customer customer) { }
}

internal sealed class TestCustomerReadRepository : ICustomerReadRepository
{
    public Task<CustomerDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult<CustomerDto?>(null);
    public Task<IReadOnlyList<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<CustomerDto>>([]);
    public Task<CustomerSummaryDto?> GetSummaryAsync(Guid customerId, CancellationToken cancellationToken = default) =>
        Task.FromResult<CustomerSummaryDto?>(null);
    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(false);
    public Task<bool> IsEmailUniqueAsync(string email, Guid? excludeCustomerId = null, CancellationToken cancellationToken = default) =>
        Task.FromResult(true);
}

internal sealed class TestInvoiceRepository : IInvoiceRepository
{
    private readonly List<Invoice> _invoices = [];
    public Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_invoices.FirstOrDefault(i => i.Id == id));
    public Task<Invoice?> GetByNumberAsync(string invoiceNumber, CancellationToken cancellationToken = default) =>
        Task.FromResult(_invoices.FirstOrDefault(i => i.InvoiceNumber == invoiceNumber));
    public Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        _invoices.Add(invoice);
        return Task.CompletedTask;
    }
    public void Remove(Invoice invoice) => _invoices.Remove(invoice);
}

internal sealed class TestInvoiceReadRepository : IInvoiceReadRepository
{
    public Task<InvoiceDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<InvoiceDto?>(null);
    public Task<InvoiceDto?> GetByNumberAsync(string invoiceNumber, CancellationToken cancellationToken = default) => Task.FromResult<InvoiceDto?>(null);
    public Task<IReadOnlyList<InvoiceListItemDto>> GetByStatusAsync(InvoiceStatus status, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<InvoiceListItemDto>>([]);
    public Task<IReadOnlyList<InvoiceListItemDto>> GetCustomerInvoicesAsync(Guid customerId, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<InvoiceListItemDto>>([]);
    public Task<IReadOnlyList<InvoiceListItemDto>> GetOverdueInvoicesAsync(DateOnly currentDate, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<InvoiceListItemDto>>([]);
    public Task<InvoiceSummaryDto> GetSummaryAsync(DateOnly currentDate, CancellationToken cancellationToken = default) =>
        Task.FromResult(new InvoiceSummaryDto(10, 2, 2, 4, 2, 10000m, 5000m, 5000m));
    public Task<PagedList<InvoiceListItemDto>> SearchAsync(InvoiceSearchParameters parameters, CancellationToken cancellationToken = default) =>
        Task.FromResult(new PagedList<InvoiceListItemDto>([], parameters.Page, parameters.PageSize, 0));
}

internal sealed class TestInvoiceNumberGenerator : IInvoiceNumberGenerator
{
    private int _count = 1;
    public Task<string> NextInvoiceNumberAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult($"INV-{_count++:D6}");
}

internal sealed class TestUnitOfWork : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(1);
}
