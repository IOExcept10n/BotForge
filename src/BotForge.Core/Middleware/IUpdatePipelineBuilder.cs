namespace BotForge.Middleware;

/// <summary>
/// Builder for configuring and constructing the update processing pipeline.
/// Implementations hold a mutable <see cref="Config"/> used by pipeline configuration extensions
/// and assemble middleware components into a single <see cref="UpdateHandler"/>.
/// </summary>
public interface IUpdatePipelineBuilder
{
    /// <summary>
    /// Gets or sets the <see cref="UpdatePipelineConfig"/> instance that contains pipeline settings
    /// (for example: maximum tracked users, lock expiration). Extensions and middleware may read
    /// or replace this value; prefer immutable config types updated via 'with' expressions.
    /// </summary>
    UpdatePipelineConfig Config { get; set; }

    /// <summary>
    /// Adds a middleware delegate to the pipeline.
    /// The provided <paramref name="middleware"/> function receives the next <see cref="UpdateHandler"/>
    /// and must return a new <see cref="UpdateHandler"/> that invokes the next delegate as appropriate.
    /// </summary>
    /// <param name="middleware">A function that composes middleware around the next delegate. Cannot be null.</param>
    /// <returns>The same <see cref="IUpdatePipelineBuilder"/> instance for fluent chaining.</returns>
    IUpdatePipelineBuilder Use(Func<UpdateHandler, UpdateHandler> middleware);

    /// <summary>
    /// Registers a middleware type that implements <see cref="IUpdateMiddleware"/>.
    /// The builder or DI container is expected to create an instance of <typeparamref name="T"/>
    /// when building the pipeline. Use this for middleware that requires constructor injection.
    /// </summary>
    /// <typeparam name="T">The middleware type that implements <see cref="IUpdateMiddleware"/>.</typeparam>
    /// <returns>The same <see cref="IUpdatePipelineBuilder"/> instance for fluent chaining.</returns>
    IUpdatePipelineBuilder UseMiddleware<T>() where T : IUpdateMiddleware;

    /// <summary>
    /// Builds the composed <see cref="UpdateHandler"/> by applying registered middleware around the
    /// provided <paramref name="terminal"/> delegate. The resulting delegate represents the full pipeline.
    /// </summary>
    /// <param name="terminal">The terminal <see cref="UpdateHandler"/> to invoke at the end of the pipeline. Cannot be null.</param>
    /// <returns>A composed <see cref="UpdateHandler"/> that executes the configured middleware chain ending with <paramref name="terminal"/>.</returns>
    UpdateHandler Build(UpdateHandler terminal);
}

/// <remarks>
/// Represents configuration for the <see cref="UpdateProcessingPipeline"/>.
/// </remarks>
/// <param name="MaxTrackedUsers">Maximal number of users for synchronization caching.</param>
/// <param name="LockExpiration">Minimal lifetime of a single user lock cache entry.</param>
public readonly record struct UpdatePipelineConfig(int MaxTrackedUsers, TimeSpan LockExpiration);

/// <summary>
/// Represents a function that asynchronously handles an update context.
/// </summary>
/// <param name="context">Context of the incoming update to handle.</param>
/// <param name="ct">An instance of the cancellation token to cancel the processing task.</param>
/// <returns>An asynchronous task to handle the incoming update.</returns>
public delegate Task UpdateHandler(UpdateContext context, CancellationToken ct);
