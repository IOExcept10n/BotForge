using BotForge.Messaging;
using BotForge.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BotForge.Hosting.Middleware;

/// <summary>
/// Middleware for logging unhandled exceptions that occur during the processing of updates in the BotForge framework.
/// </summary>
public partial class ErrorLoggingMiddleware : IUpdateMiddleware
{
    /// <inheritdoc/>
    public async Task InvokeAsync(UpdateContext context, Func<Task> nextStep, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(nextStep);
        try
        {
            await nextStep().ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is not TaskCanceledException)
        {
            var logger = context.Services.GetRequiredService<ILogger<ErrorLoggingMiddleware>>();

            if (!logger.IsEnabled(LogLevel.Error))
                return;

            if (context.Update.IsMessage)
                LogMessageError(logger, ex, context.Update.Message);
            else if (context.Update.IsInteraction)
                LogInteractionError(logger, ex, context.Update.Interaction);
            else
                LogError(logger, ex, context.Update.RawUpdate ?? context.Update);
        }
    }

    [LoggerMessage(LogLevel.Error, Message = "An unhandled exception when processing the update message {Content}")]
    private static partial void LogMessageError(ILogger logger, Exception ex, IMessage Content);

    [LoggerMessage(LogLevel.Error, Message = "An unhandled exception when processing the update interaction {Content}")]
    private static partial void LogInteractionError(ILogger logger, Exception ex, IInteraction Content);

    [LoggerMessage(LogLevel.Error, Message = "An unhandled exception when processing update {Update}")]
    private static partial void LogError(ILogger logger, Exception ex, object Update);
}
