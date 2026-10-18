using Dapper;
using InvoiceApp.Domain.Models;
using InvoiceApp.Domain.Repositories;
using InvoiceApp.Infrastructure.Connection;

namespace InvoiceApp.Infrastructure.Persistence.Repositories;

internal sealed class CustomerReadRepository : ICustomerReadRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CustomerReadRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<CustomerDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT 
                Id,
                Name,
                Email,
                Phone,
                Address,
                CreatedAtUtc,
                UpdatedAtUtc
            FROM Customers
            WHERE Id = @Id;
            """;

        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        return await connection.QueryFirstOrDefaultAsync<CustomerDto>(new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyList<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT 
                Id,
                Name,
                Email,
                Phone,
                Address,
                CreatedAtUtc,
                UpdatedAtUtc
            FROM Customers
            ORDER BY Name ASC;
            """;

        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var customers = await connection.QueryAsync<CustomerDto>(new CommandDefinition(sql, cancellationToken: cancellationToken));
        return customers.ToList();
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT CASE WHEN EXISTS (SELECT 1 FROM Customers WHERE Id = @Id) THEN 1 ELSE 0 END;";

        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        return await connection.ExecuteScalarAsync<bool>(new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<bool> IsEmailUniqueAsync(string email, Guid? excludeCustomerId = null, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT CASE WHEN NOT EXISTS (
                SELECT 1 FROM Customers
                WHERE LOWER(Email) = LOWER(@Email)
                  AND (@ExcludeCustomerId IS NULL OR Id <> @ExcludeCustomerId)
            ) THEN 1 ELSE 0 END;
            """;

        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        return await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(sql, new { Email = email.Trim(), ExcludeCustomerId = excludeCustomerId }, cancellationToken: cancellationToken));
    }

    public async Task<CustomerSummaryDto?> GetSummaryAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                c.Id AS CustomerId,
                c.Name AS CustomerName,
                COUNT(i.Id) AS TotalInvoices,
                COALESCE(SUM(CASE WHEN i.Status = 'Paid' THEN 1 ELSE 0 END), 0) AS PaidInvoices,
                COALESCE(SUM(CASE WHEN i.Status IN ('Draft', 'Sent', 'Overdue') THEN 1 ELSE 0 END), 0) AS OutstandingInvoices,
                COALESCE(SUM(inv_totals.total_amount), 0) AS TotalInvoiced,
                COALESCE(SUM(CASE WHEN i.Status = 'Paid' THEN inv_totals.total_amount ELSE 0 END), 0) AS TotalPaid,
                COALESCE(SUM(CASE WHEN i.Status IN ('Draft', 'Sent', 'Overdue') THEN inv_totals.total_amount ELSE 0 END), 0) AS TotalOutstanding
            FROM Customers c
            LEFT JOIN Invoices i ON i.CustomerId = c.Id
            LEFT JOIN (
                SELECT InvoiceId, SUM(Quantity * UnitPrice) AS total_amount
                FROM InvoiceItems
                GROUP BY InvoiceId
            ) inv_totals ON inv_totals.InvoiceId = i.Id
            WHERE c.Id = @CustomerId
            GROUP BY c.Id, c.Name;
            """;

        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        return await connection.QueryFirstOrDefaultAsync<CustomerSummaryDto>(
            new CommandDefinition(sql, new { CustomerId = customerId }, cancellationToken: cancellationToken));
    }
}
