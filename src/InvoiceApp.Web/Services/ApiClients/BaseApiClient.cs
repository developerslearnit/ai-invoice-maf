using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using InvoiceApp.Web.Models.Api;

namespace InvoiceApp.Web.Services.ApiClients;

public abstract class BaseApiClient
{
    protected readonly HttpClient HttpClient;
    protected readonly ILogger Logger;

    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    protected BaseApiClient(HttpClient httpClient, ILogger logger)
    {
        HttpClient = httpClient;
        Logger = logger;
    }

    protected async Task<ApiResponse<T>> GetAsync<T>(string uri, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await HttpClient.GetAsync(uri, cancellationToken);
            return await HandleResponseAsync<T>(response, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            Logger.LogError(ex, "HTTP request failure calling {Uri}", uri);
            return ApiResponse<T>.Failure($"Unable to communicate with the Invoice API at {HttpClient.BaseAddress}. Please verify the API is running.", 503);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            Logger.LogError(ex, "Request timed out calling {Uri}", uri);
            return ApiResponse<T>.Failure("Request to the Invoice API timed out.", 504);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unexpected error calling {Uri}", uri);
            return ApiResponse<T>.Failure("An unexpected error occurred while communicating with the API.", 500);
        }
    }

    protected async Task<ApiResponse<TResult>> PostAsJsonAsync<TRequest, TResult>(
        string uri,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await HttpClient.PostAsJsonAsync(uri, request, JsonOptions, cancellationToken);
            return await HandleResponseAsync<TResult>(response, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            Logger.LogError(ex, "HTTP request failure calling POST {Uri}", uri);
            return ApiResponse<TResult>.Failure("Unable to connect to the backend Invoice API.", 503);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unexpected error calling POST {Uri}", uri);
            return ApiResponse<TResult>.Failure("An unexpected error occurred.", 500);
        }
    }

    protected async Task<ApiResponse<bool>> PutAsJsonAsync<TRequest>(
        string uri,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await HttpClient.PutAsJsonAsync(uri, request, JsonOptions, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return ApiResponse<bool>.Success(true, (int)response.StatusCode);
            }

            return await HandleFailureAsync<bool>(response, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unexpected error calling PUT {Uri}", uri);
            return ApiResponse<bool>.Failure("Unable to update resource. Ensure the API is reachable.", 500);
        }
    }

    protected async Task<ApiResponse<bool>> PatchAsJsonAsync<TRequest>(
        string uri,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await HttpClient.PatchAsJsonAsync(uri, request, JsonOptions, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return ApiResponse<bool>.Success(true, (int)response.StatusCode);
            }

            return await HandleFailureAsync<bool>(response, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unexpected error calling PATCH {Uri}", uri);
            return ApiResponse<bool>.Failure("Unable to update resource status.", 500);
        }
    }

    protected async Task<ApiResponse<bool>> DeleteAsync(string uri, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await HttpClient.DeleteAsync(uri, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return ApiResponse<bool>.Success(true, (int)response.StatusCode);
            }

            return await HandleFailureAsync<bool>(response, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unexpected error calling DELETE {Uri}", uri);
            return ApiResponse<bool>.Failure("Unable to delete resource.", 500);
        }
    }

    private static async Task<ApiResponse<T>> HandleResponseAsync<T>(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            if (typeof(T) == typeof(bool))
            {
                return (ApiResponse<T>)(object)ApiResponse<bool>.Success(true, (int)response.StatusCode);
            }

            var data = await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);
            return data is not null
                ? ApiResponse<T>.Success(data, (int)response.StatusCode)
                : ApiResponse<T>.Failure("API returned an empty payload.", (int)response.StatusCode);
        }

        return await HandleFailureAsync<T>(response, cancellationToken);
    }

    private static async Task<ApiResponse<T>> HandleFailureAsync<T>(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var statusCode = (int)response.StatusCode;
        try
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(content))
            {
                return ApiResponse<T>.Failure($"API responded with status {response.ReasonPhrase} ({statusCode}).", statusCode);
            }

            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            // Check if Validation Problem (errors dictionary)
            if (root.TryGetProperty("errors", out var errorsProp) && errorsProp.ValueKind == JsonValueKind.Object)
            {
                var errors = new Dictionary<string, string[]>();
                foreach (var prop in errorsProp.EnumerateObject())
                {
                    var errorList = prop.Value.EnumerateArray().Select(e => e.GetString() ?? "").Where(e => !string.IsNullOrEmpty(e)).ToArray();
                    errors[prop.Name] = errorList;
                }

                var title = root.TryGetProperty("title", out var titleProp) ? titleProp.GetString() : "Validation failed.";
                return ApiResponse<T>.Failure(title ?? "Validation failed.", statusCode, errors);
            }

            // Check standard ProblemDetails (detail or title)
            if (root.TryGetProperty("detail", out var detailProp) && detailProp.GetString() is { Length: > 0 } detail)
            {
                return ApiResponse<T>.Failure(detail, statusCode);
            }

            if (root.TryGetProperty("title", out var titleOnlyProp) && titleOnlyProp.GetString() is { Length: > 0 } titleOnly)
            {
                return ApiResponse<T>.Failure(titleOnly, statusCode);
            }

            return ApiResponse<T>.Failure(content, statusCode);
        }
        catch
        {
            return ApiResponse<T>.Failure($"API request failed with status code {statusCode}.", statusCode);
        }
    }
}
