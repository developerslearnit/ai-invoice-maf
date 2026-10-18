using Dapper;
using InvoiceApp.Domain.Services;
using InvoiceApp.Infrastructure.Connection;

namespace InvoiceApp.Infrastructure.Services;

public sealed class SqlServerInvoiceNumberGenerator : IInvoiceNumberGenerator
{
    private readonly IDbConnectionFactory _connectionFactory;

    public SqlServerInvoiceNumberGenerator(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<string> NextInvoiceNumberAsync(CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT NEXT VALUE FOR invoice_number_seq;";

        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        try
        {
            var nextVal = await connection.ExecuteScalarAsync<long>(new CommandDefinition(sql, cancellationToken: cancellationToken));
            return $"INV-{nextVal:D6}";
        }
        catch
        {
            // Fallback if sequence is not yet initialized: query max number and increment
            const string fallbackSql = """
                SELECT TOP 1 InvoiceNumber 
                FROM Invoices 
                WHERE InvoiceNumber LIKE 'INV-%' 
                ORDER BY InvoiceNumber DESC;
                """;

            var lastNumber = await connection.ExecuteScalarAsync<string?>(new CommandDefinition(fallbackSql, cancellationToken: cancellationToken));
            var nextNum = 1;
            if (lastNumber is not null && lastNumber.StartsWith("INV-", StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(lastNumber[4..], out var parsed))
                {
                    nextNum = parsed + 1;
                }
            }

            return $"INV-{nextNum:D6}";
        }
    }
}
