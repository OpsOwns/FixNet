using FixNet.Domain.Base;

namespace FixNet.API.Utilities;

public static class ResultExtension
{
    public static IResult ToProblemResult(this Result result, string path)
    {
        if (!result.IsFailure)
        {
            throw new InvalidOperationException("The result must represent a failure to create a validation problem response.");
        }

        var httpProblemDetails = new HttpValidationProblemDetails(new Dictionary<string, string[]>
        {
            { result.Error.Code, [result.Error.Message] }
        })
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Status = StatusCodes.Status400BadRequest,
            Title = "Business Rule Violation",
            Detail = "One or more business rules have been violated.",
            Instance = path
        };

        return Results.Problem(httpProblemDetails);
    }
}