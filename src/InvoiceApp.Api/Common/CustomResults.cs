using FluentValidation.Results;
using InvoiceApp.Domain.Common;
using Microsoft.AspNetCore.Http;

namespace InvoiceApp.Api.Common;

public static class CustomResults
{
    public static IResult Problem(Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status422UnprocessableEntity
        };

        var typeUrl = error.Type switch
        {
            ErrorType.Validation => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            ErrorType.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            ErrorType.Conflict => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
            ErrorType.Unauthorized => "https://tools.ietf.org/html/rfc7235#section-3.1",
            ErrorType.Forbidden => "https://tools.ietf.org/html/rfc7231#section-6.5.3",
            _ => "https://tools.ietf.org/html/rfc4918#section-11.2"
        };

        return Results.Problem(
            statusCode: statusCode,
            title: error.Code,
            detail: error.Description,
            type: typeUrl);
    }

    public static IResult ValidationProblem(ValidationResult validationResult)
    {
        var errors = validationResult.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => JsonNamingPolicyCamelCase(g.Key),
                g => g.Select(e => e.ErrorMessage).ToArray());

        return Results.ValidationProblem(
            errors: errors,
            title: "Validation failed",
            statusCode: StatusCodes.Status400BadRequest,
            type: "https://httpstatuses.com/400");
    }

    private static string JsonNamingPolicyCamelCase(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            return key;
        }

        return char.ToLowerInvariant(key[0]) + key[1..];
    }
}
