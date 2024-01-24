using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace CrossCutting.Logging.Extensions;

public static class LoggingExtensions
{
    public static void LogErrorWithPushProperty<T>(
        this ILogger<T> logger,
        string name,
        object? value,
        string messageTemplate,
        string? message,
        bool destructureObjects = true)
    {
        using (LogContext.PushProperty(name, value, destructureObjects))
        {
            logger.LogError(messageTemplate, message);
        }
    }
}
