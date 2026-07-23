using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FixNet.API.Middleware;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(
            exception,
            "FixNet Exception at {Path}",
            httpContext.Request.Path);

        const string internalErrorUrl =
            "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1";
        const string badRequestUrl =
            "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1";
        const string unauthorizedUrl =
            "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1";
        const string notFoundUrl =
            "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4";

        var (statusCode, title, type, detail) = exception switch
        {
            UnauthorizedAccessException =>
            (
                StatusCodes.Status401Unauthorized,
                "Unauthorized Access",
                unauthorizedUrl,
                exception.Message
            ),

            InvalidOperationException =>
            (
                StatusCodes.Status400BadRequest,
                "Invalid Operation",
                badRequestUrl,
                exception.Message
            ),

            KeyNotFoundException =>
            (
                StatusCodes.Status404NotFound,
                "Resource Not Found",
                notFoundUrl,
                exception.Message
            ),
            _ =>
            (
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                internalErrorUrl,
                "An unexpected error occurred."
            )
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path,
            Type = type,
            Extensions =
            {
                ["traceId"] = httpContext.TraceIdentifier
            }
        };

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);

        return true;
    }
}