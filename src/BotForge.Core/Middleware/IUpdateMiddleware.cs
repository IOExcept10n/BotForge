using BotForge.Messaging;

namespace BotForge.Middleware;

/// <summary>
/// Middleware invoked during update processing. Middleware can inspect or modify the <see cref="UpdateContext"/>,
/// perform side effects, and decide whether to call the next delegate in the pipeline.
/// </summary>
public interface IUpdateMiddleware
{
    /// <summary>
    /// Invokes the middleware logic.
    /// </summary>
    /// <param name="context">The update context containing the incoming update and services. Cannot be null.</param>
    /// <param name="nextStep">A delegate that invokes the next middleware in the pipeline. Middleware must call this to continue the chain.</param>
    /// <param name="ct">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous middleware operation.</returns>
    Task InvokeAsync(UpdateContext context, Func<Task> nextStep, CancellationToken ct);
}

/// <summary>
/// Context passed to middleware and other pipeline components containing the update being processed
/// and an <see cref="IServiceProvider"/> for resolving services.
/// </summary>
/// <param name="Update">The update being processed.</param>
/// <param name="Services">The service provider used for resolving services during processing.</param>
public sealed record UpdateContext(IUpdate Update, IServiceProvider Services);
