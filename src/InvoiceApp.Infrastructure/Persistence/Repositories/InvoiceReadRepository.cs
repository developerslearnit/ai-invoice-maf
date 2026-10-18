using System.Text;
using Dapper;
using InvoiceApp.Domain.Enums;
using InvoiceApp.Domain.Models;
using InvoiceApp.Domain.Repositories;
using InvoiceApp.Infrastructure.Connection;

namespace InvoiceApp.Infrastructure.Persistence.Repositories;

internal sealed class InvoiceReadRepository : IInvoiceReadRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public InvoiceReadRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<InvoiceDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await GetInvoiceWithDetailsAsync("i.Id = @Id", new { Id = id }, cancellationToken);
    }

    public async Task<InvoiceDto?> GetByNumberAsync(string invoiceNumber, CancellationToken cancellationToken = default)
    {
        return await GetInvoiceWithDetailsAsync("LOWER(i.InvoiceNumber) = LOWER(@InvoiceNumber)", new { InvoiceNumber = invoiceNumber }, cancellationToken);
    }

    private async Task<InvoiceDto?> GetInvoiceWithDetailsAsync(string whereClause, object parameters, CancellationToken cancellationToken)
    {
        var sql = $"""
            SELECT 
                i.Id AS Id,
                i.InvoiceNumber AS InvoiceNumber,
                i.IssueDate AS IssueDate,
                i.DueDate AS DueDate,
                i.Status AS StatusStr,
                i.Notes AS Notes,
                c.Id AS CustomerId,
                c.Name AS CustomerName,
                c.Email AS CustomerEmail,
                c.Phone AS CustomerPhone,
                c.Address AS CustomerAddress,
                c.CreatedAtUtc AS CustomerCreatedAtUtc,
                c.UpdatedAtUtc AS CustomerUpdatedAtUtc
            FROM Invoices i
            INNER JOIN Customers c ON c.Id = i.CustomerId
            WHERE {whereClause};

            SELECT 
                item.Id AS Id,
                item.Description AS Description,
                item.Quantity AS Quantity,
                item.UnitPrice AS UnitPrice,
                (item.Quantity * item.UnitPrice) AS LineTotal
            FROM InvoiceItems item
            INNER JOIN Invoices i ON i.Id = item.InvoiceId
            WHERE {whereClause}
            ORDER BY item.Id;
            """;

        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        using var multi = await connection.QueryMultipleAsync(new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

        var invoiceRow = await multi.ReadFirstOrDefaultAsync<dynamic>();
        if (invoiceRow is null)
        {
            return null;
        }

        var items = (await multi.ReadAsync<InvoiceItemDto>()).ToList();
        var rowDict = (IDictionary<string, object>)invoiceRow;

        var customer = new CustomerDto(
            (Guid)GetVal(rowDict, "CustomerId", "customerid"),
            (string)GetVal(rowDict, "CustomerName", "customername"),
            (string)GetVal(rowDict, "CustomerEmail", "customeremail"),
            (string?)GetNullableVal(rowDict, "CustomerPhone", "customerphone"),
            (string?)GetNullableVal(rowDict, "CustomerAddress", "customeraddress"),
            Convert.ToDateTime(GetVal(rowDict, "CustomerCreatedAtUtc", "customercreatedatutc")),
            GetNullableVal(rowDict, "CustomerUpdatedAtUtc", "customerupdatedatutc") is { } u ? Convert.ToDateTime(u) : null);

        var statusStr = (string)GetVal(rowDict, "StatusStr", "statusstr");
        var status = Enum.Parse<InvoiceStatus>(statusStr);
        var totalAmount = items.Sum(item => item.LineTotal);

        var issueDateVal = GetVal(rowDict, "IssueDate", "issuedate");
        var dueDateVal = GetVal(rowDict, "DueDate", "duedate");

        return new InvoiceDto(
            (Guid)GetVal(rowDict, "Id", "id"),
            (string)GetVal(rowDict, "InvoiceNumber", "invoicenumber"),
            customer,
            issueDateVal is DateOnly d1 ? d1 : DateOnly.FromDateTime(Convert.ToDateTime(issueDateVal)),
            dueDateVal is DateOnly d2 ? d2 : DateOnly.FromDateTime(Convert.ToDateTime(dueDateVal)),
            status,
            (string?)GetNullableVal(rowDict, "Notes", "notes"),
            totalAmount,
            items);
    }

    public async Task<IReadOnlyList<InvoiceListItemDto>> GetByStatusAsync(InvoiceStatus status, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT 
                i.Id AS Id,
                i.InvoiceNumber AS InvoiceNumber,
                i.CustomerId AS CustomerId,
                c.Name AS CustomerName,
                i.IssueDate AS IssueDate,
                i.DueDate AS DueDate,
                i.Status AS Status,
                COALESCE(inv_totals.total_amount, 0) AS TotalAmount
            FROM Invoices i
            INNER JOIN Customers c ON c.Id = i.CustomerId
            LEFT JOIN (
                SELECT InvoiceId, SUM(Quantity * UnitPrice) AS total_amount
                FROM InvoiceItems
                GROUP BY InvoiceId
            ) inv_totals ON inv_totals.InvoiceId = i.Id
            WHERE i.Status = @Status
            ORDER BY i.CreatedAtUtc DESC;
            """;

        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var rows = await connection.QueryAsync<dynamic>(new CommandDefinition(sql, new { Status = status.ToString() }, cancellationToken: cancellationToken));
        return MapInvoiceListItems(rows);
    }

    public async Task<IReadOnlyList<InvoiceListItemDto>> GetCustomerInvoicesAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT 
                i.Id AS Id,
                i.InvoiceNumber AS InvoiceNumber,
                i.CustomerId AS CustomerId,
                c.Name AS CustomerName,
                i.IssueDate AS IssueDate,
                i.DueDate AS DueDate,
                i.Status AS Status,
                COALESCE(inv_totals.total_amount, 0) AS TotalAmount
            FROM Invoices i
            INNER JOIN Customers c ON c.Id = i.CustomerId
            LEFT JOIN (
                SELECT InvoiceId, SUM(Quantity * UnitPrice) AS total_amount
                FROM InvoiceItems
                GROUP BY InvoiceId
            ) inv_totals ON inv_totals.InvoiceId = i.Id
            WHERE i.CustomerId = @CustomerId
            ORDER BY i.CreatedAtUtc DESC;
            """;

        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var rows = await connection.QueryAsync<dynamic>(new CommandDefinition(sql, new { CustomerId = customerId }, cancellationToken: cancellationToken));
        return MapInvoiceListItems(rows);
    }

    public async Task<IReadOnlyList<InvoiceListItemDto>> GetOverdueInvoicesAsync(DateOnly currentDate, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT 
                i.Id AS Id,
                i.InvoiceNumber AS InvoiceNumber,
                i.CustomerId AS CustomerId,
                c.Name AS CustomerName,
                i.IssueDate AS IssueDate,
                i.DueDate AS DueDate,
                i.Status AS Status,
                COALESCE(inv_totals.total_amount, 0) AS TotalAmount
            FROM Invoices i
            INNER JOIN Customers c ON c.Id = i.CustomerId
            LEFT JOIN (
                SELECT InvoiceId, SUM(Quantity * UnitPrice) AS total_amount
                FROM InvoiceItems
                GROUP BY InvoiceId
            ) inv_totals ON inv_totals.InvoiceId = i.Id
            WHERE i.DueDate < @CurrentDate
              AND i.Status NOT IN ('Paid', 'Cancelled')
            ORDER BY i.DueDate ASC;
            """;

        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var rows = await connection.QueryAsync<dynamic>(
            new CommandDefinition(sql, new { CurrentDate = currentDate.ToDateTime(TimeOnly.MinValue) }, cancellationToken: cancellationToken));
        return MapInvoiceListItems(rows);
    }

    public async Task<InvoiceSummaryDto> GetSummaryAsync(DateOnly currentDate, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                COUNT(i.Id) AS TotalInvoices,
                COALESCE(SUM(CASE WHEN i.Status = 'Draft' THEN 1 ELSE 0 END), 0) AS DraftInvoices,
                COALESCE(SUM(CASE WHEN i.Status = 'Sent' THEN 1 ELSE 0 END), 0) AS SentInvoices,
                COALESCE(SUM(CASE WHEN i.Status = 'Paid' THEN 1 ELSE 0 END), 0) AS PaidInvoices,
                COALESCE(SUM(CASE WHEN i.Status = 'Overdue' THEN 1 ELSE 0 END), 0) AS OverdueInvoices,
                COALESCE(SUM(inv_totals.total_amount), 0) AS TotalInvoiceAmount,
                COALESCE(SUM(CASE WHEN i.Status = 'Paid' THEN inv_totals.total_amount ELSE 0 END), 0) AS TotalPaidAmount,
                COALESCE(SUM(CASE WHEN i.Status IN ('Draft', 'Sent', 'Overdue') THEN inv_totals.total_amount ELSE 0 END), 0) AS TotalOutstandingAmount
            FROM Invoices i
            LEFT JOIN (
                SELECT InvoiceId, SUM(Quantity * UnitPrice) AS total_amount
                FROM InvoiceItems
                GROUP BY InvoiceId
            ) inv_totals ON inv_totals.InvoiceId = i.Id;
            """;

        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var summary = await connection.QueryFirstOrDefaultAsync<InvoiceSummaryDto>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));

        return summary ?? new InvoiceSummaryDto(0, 0, 0, 0, 0, 0, 0, 0);
    }

    public async Task<PagedList<InvoiceListItemDto>> SearchAsync(InvoiceSearchParameters parameters, CancellationToken cancellationToken = default)
    {
        var where = new StringBuilder("WHERE 1=1");
        var dp = new DynamicParameters();

        if (parameters.Status.HasValue)
        {
            where.Append(" AND i.Status = @Status");
            dp.Add("Status", parameters.Status.Value.ToString());
        }

        if (parameters.CustomerId.HasValue)
        {
            where.Append(" AND i.CustomerId = @CustomerId");
            dp.Add("CustomerId", parameters.CustomerId.Value);
        }

        if (parameters.FromDate.HasValue)
        {
            where.Append(" AND i.IssueDate >= @FromDate");
            dp.Add("FromDate", parameters.FromDate.Value.ToDateTime(TimeOnly.MinValue));
        }

        if (parameters.ToDate.HasValue)
        {
            where.Append(" AND i.IssueDate <= @ToDate");
            dp.Add("ToDate", parameters.ToDate.Value.ToDateTime(TimeOnly.MinValue));
        }

        if (parameters.MinimumAmount.HasValue)
        {
            where.Append(" AND COALESCE(inv_totals.total_amount, 0) >= @MinimumAmount");
            dp.Add("MinimumAmount", parameters.MinimumAmount.Value);
        }

        if (parameters.MaximumAmount.HasValue)
        {
            where.Append(" AND COALESCE(inv_totals.total_amount, 0) <= @MaximumAmount");
            dp.Add("MaximumAmount", parameters.MaximumAmount.Value);
        }

        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            where.Append(" AND (i.InvoiceNumber LIKE @SearchPattern OR c.Name LIKE @SearchPattern)");
            dp.Add("SearchPattern", $"%{parameters.Search.Trim()}%");
        }

        var countSql = $"""
            SELECT COUNT(i.Id)
            FROM Invoices i
            INNER JOIN Customers c ON c.Id = i.CustomerId
            LEFT JOIN (
                SELECT InvoiceId, SUM(Quantity * UnitPrice) AS total_amount
                FROM InvoiceItems
                GROUP BY InvoiceId
            ) inv_totals ON inv_totals.InvoiceId = i.Id
            {where};
            """;

        var offset = (parameters.Page - 1) * parameters.PageSize;
        dp.Add("Offset", offset);
        dp.Add("Limit", parameters.PageSize);

        var dataSql = $"""
            SELECT 
                i.Id AS Id,
                i.InvoiceNumber AS InvoiceNumber,
                i.CustomerId AS CustomerId,
                c.Name AS CustomerName,
                i.IssueDate AS IssueDate,
                i.DueDate AS DueDate,
                i.Status AS Status,
                COALESCE(inv_totals.total_amount, 0) AS TotalAmount
            FROM Invoices i
            INNER JOIN Customers c ON c.Id = i.CustomerId
            LEFT JOIN (
                SELECT InvoiceId, SUM(Quantity * UnitPrice) AS total_amount
                FROM InvoiceItems
                GROUP BY InvoiceId
            ) inv_totals ON inv_totals.InvoiceId = i.Id
            {where}
            ORDER BY i.CreatedAtUtc DESC
            OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY;
            """;

        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var totalCount = await connection.ExecuteScalarAsync<int>(new CommandDefinition(countSql, dp, cancellationToken: cancellationToken));
        var rows = await connection.QueryAsync<dynamic>(new CommandDefinition(dataSql, dp, cancellationToken: cancellationToken));

        var items = MapInvoiceListItems(rows);

        return new PagedList<InvoiceListItemDto>(items, parameters.Page, parameters.PageSize, totalCount);
    }

    private static List<InvoiceListItemDto> MapInvoiceListItems(IEnumerable<dynamic> rows)
    {
        return rows.Select(r =>
        {
            var rowDict = (IDictionary<string, object>)r;
            var issueDate = GetVal(rowDict, "IssueDate", "issuedate");
            var dueDate = GetVal(rowDict, "DueDate", "duedate");
            return new InvoiceListItemDto(
                (Guid)GetVal(rowDict, "Id", "id"),
                (string)GetVal(rowDict, "InvoiceNumber", "invoicenumber"),
                (Guid)GetVal(rowDict, "CustomerId", "customerid"),
                (string)GetVal(rowDict, "CustomerName", "customername"),
                issueDate is DateOnly d1 ? d1 : DateOnly.FromDateTime(Convert.ToDateTime(issueDate)),
                dueDate is DateOnly d2 ? d2 : DateOnly.FromDateTime(Convert.ToDateTime(dueDate)),
                Enum.Parse<InvoiceStatus>((string)GetVal(rowDict, "Status", "status")),
                Convert.ToDecimal(GetVal(rowDict, "TotalAmount", "totalamount")));
        }).ToList();
    }

    private static object GetVal(IDictionary<string, object> dict, string key1, string key2)
    {
        if (dict.TryGetValue(key1, out var val1) && val1 != null) return val1;
        if (dict.TryGetValue(key2, out var val2) && val2 != null) return val2;
        throw new KeyNotFoundException($"Neither key '{key1}' nor '{key2}' was found in the row dictionary.");
    }

    private static object? GetNullableVal(IDictionary<string, object> dict, string key1, string key2)
    {
        if (dict.TryGetValue(key1, out var val1)) return val1;
        if (dict.TryGetValue(key2, out var val2)) return val2;
        return null;
    }
}
