using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace API.Utilities;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogGlobalError(exception.Message, httpContext.Request.Path, exception);

        const string internalErrorUrl = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1";
        const string badRequestUrl = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1";

        var (statusCode, title, type) = exception switch
        {
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized Access", badRequestUrl),
            InvalidOperationException => (StatusCodes.Status400BadRequest, "Invalid FixNet Operation", badRequestUrl),
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource Not Found", badRequestUrl),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error", internalErrorUrl)
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = exception.Message,
            Instance = httpContext.Request.Path,
            Type = type
        };

        problemDetails.Extensions.Add("traceId", httpContext.TraceIdentifier);

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}