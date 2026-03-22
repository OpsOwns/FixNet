using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace FixNet.API.Utilities;

public static class FixNetRateLimiter
{
    public const string PolicyName = "FixNetPolicy";

    public static void AddFixNetPolicy(this RateLimiterOptions options)
    {
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        options.AddPolicy(PolicyName, httpContext =>
        {
            var path = httpContext.Request.Path.Value?.ToLower(System.Globalization.CultureInfo.InvariantCulture) ?? "";

            if (path.Contains("/client"))
            {
                return RateLimitPartition.GetFixedWindowLimiter("ClientPartition", _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 5,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0
                });
            }

            if (path.Contains("/technic"))
            {
                return RateLimitPartition.GetFixedWindowLimiter("TechnicianPartition", _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 20,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 2
                });
            }

            return RateLimitPartition.GetFixedWindowLimiter("GlobalKey", _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 50,
                Window = TimeSpan.FromMinutes(1)
            });
        });
    }
}