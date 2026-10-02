using Microsoft.Extensions.Logging;

namespace Rag_Console_App_Source.Infrastructure.Logging;

public static class LoggingExtensions
{
    public static void LogOperationStart(this ILogger logger, string operationName)
        => logger.LogInformation("Starting: {OperationName}", operationName);

    public static void LogOperationEnd(this ILogger logger, string operationName)
        => logger.LogInformation("Completed: {OperationName}", operationName);

    public static void LogOperationError(this ILogger logger, string operationName, Exception ex)
        => logger.LogError(ex, "Error during: {OperationName}", operationName);
}
