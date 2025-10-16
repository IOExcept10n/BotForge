namespace BotForge.Middleware;

public interface IUpdatePipelineBuilder
{
    UpdatePipelineConfig Config { get; set; }

    IUpdatePipelineBuilder Use(Func<UpdateDelegate, UpdateDelegate> middleware);

    IUpdatePipelineBuilder UseMiddleware<T>() where T : IUpdateMiddleware;

    UpdateDelegate Build(UpdateDelegate terminal);
}

/// <remarks>
/// Represents configuration for the <see cref="UpdateProcessingPipeline"/>.
/// </remarks>
/// <param name="MaxTrackedUsers">Maximal number of users for synchronization caching.</param>
/// <param name="LockExpiration">Minimal lifetime of a single user lock cache entry.</param>
public readonly record struct UpdatePipelineConfig(int MaxTrackedUsers, TimeSpan LockExpiration);

public delegate Task UpdateDelegate(UpdateContext context, CancellationToken ct);
