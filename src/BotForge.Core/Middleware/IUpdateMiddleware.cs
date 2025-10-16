using BotForge.Core.Messaging;

namespace BotForge.Core.Middleware;
public interface IUpdateMiddleware
{
    Task InvokeAsync(UpdateContext context, Func<Task> next, CancellationToken ct);
}

public sealed record UpdateContext(IUpdate Update, IServiceProvider Services);
