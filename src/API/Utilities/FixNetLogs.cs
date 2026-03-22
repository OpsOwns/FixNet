namespace API.Utilities;

public static partial class FixNetLogs
{
    [LoggerMessage(Level = LogLevel.Error, Message = "FixNet Exception: {Message} at {Path}")]
    public static partial void LogGlobalError(this ILogger logger, string message, string path, Exception ex);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Rate Limit Exceeded for {Path} from IP: {Ip}")]
    public static partial void LogRateLimit(this ILogger logger, string path, string ip);

    [LoggerMessage(Level = LogLevel.Information, Message = "Cache Miss for key: {Key}")]
    public static partial void LogCacheMiss(this ILogger logger, string key);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Request cancelled during execution for idempotency key: {Key}")]
    public static partial void LogIdempotencyCancelled(this ILogger logger, string key);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Idempotency hit for key {Key}")]
    public static partial void LogIdempotencyHit(this ILogger logger, string key);
}