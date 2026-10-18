namespace InvoiceApp.Web.Models.Api;

public sealed record PagedListApiModel<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalCount)
{
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}

public sealed class ApiResponse<T>
{
    public bool IsSuccess { get; init; }
    public T? Data { get; init; }
    public string? ErrorMessage { get; init; }
    public int StatusCode { get; init; }
    public IDictionary<string, string[]>? ValidationErrors { get; init; }

    public static ApiResponse<T> Success(T data, int statusCode = 200) =>
        new() { IsSuccess = true, Data = data, StatusCode = statusCode };

    public static ApiResponse<T> Failure(string errorMessage, int statusCode, IDictionary<string, string[]>? validationErrors = null) =>
        new() { IsSuccess = false, ErrorMessage = errorMessage, StatusCode = statusCode, ValidationErrors = validationErrors };
}
