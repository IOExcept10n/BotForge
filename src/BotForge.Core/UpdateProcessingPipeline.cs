using System.Collections.Concurrent;
using BotForge.Fsm;
using BotForge.Messaging;
using BotForge.Middleware;

namespace BotForge;

/// <summary>
/// Represents a thread-safe per-user implementation of the bot updates processing pipeline. It asynchronously handles all updates from users
/// </summary>
public sealed partial class UpdateProcessingPipeline : IDisposable
{
    private readonly FsmEngine _fsm;
    private readonly ConcurrentDictionary<long, (SemaphoreSlim, DateTime)> _userLocks = new();
    private readonly int _maxTrackedUsers;
    private readonly TimeSpan _lockExpiration;
    private readonly object _cleanupLock = new();
    private readonly UpdateDelegate _pipeline;
    private readonly IServiceProvider _services;

    /// <param name="fsm">An instance of the finite state machine engine to handle updates.</param>
    /// <param name="services">An instance of the service provider to get services from.</param>
    /// <param name="configure">Action for pipeline configuration.</param>
    /// <param name="maxTrackedUsers">Maximal number of users for synchronization caching.</param>
    /// <param name="lockExpiration">Minimal lifetime of a single user lock cache entry.</param>
    public UpdateProcessingPipeline(
        FsmEngine fsm,
        IServiceProvider services,
        Action<IUpdatePipelineBuilder>? configure = null,
        int maxTrackedUsers = 10_000,
        TimeSpan? lockExpiration = null)
    {
        _fsm = fsm;
        _services = services;
        _maxTrackedUsers = maxTrackedUsers;
        _lockExpiration = lockExpiration ?? TimeSpan.FromMinutes(15);

        // Build the pipeline.
        var builder = new UpdatePipelineBuilder();
        configure?.Invoke(builder);
        _pipeline = builder.Build(async (ctx, ct) => await _fsm.HandleAsync(ctx.Update, ct).ConfigureAwait(false));
    }

    /// <summary>
    /// Main pipeline entry point. Allows parallel handling for messages from different users.
    /// </summary>
    public async Task HandleUpdateAsync(IUpdate update, CancellationToken cancellationToken = default)
    {
        if (update?.Sender == null)
        {
            return;
        }

        var userId = update.Sender.Id;
        var (semaphore, _) = GetOrCreateSemaphore(userId);

        await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var ctx = new UpdateContext(update, _services);
            await _pipeline(ctx, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            semaphore.Release();
        }
    }

    /// <summary>
    /// Gets or creates a semaphore for the specified user.
    /// Automatically clears dictionary when number of semaphores reaches limit.
    /// </summary>
    private (SemaphoreSlim, DateTime) GetOrCreateSemaphore(long userId)
    {
        if (_userLocks.TryGetValue(userId, out var semaphore))
            return semaphore;

        semaphore = _userLocks.GetOrAdd(userId, static _ => (new SemaphoreSlim(1, 1), DateTime.UtcNow));

        // Periodical cleanup when semaphores hits limit.
        if (_userLocks.Count > _maxTrackedUsers)
            CleanupInactiveUsers();

        return semaphore;
    }

    /// <summary>
    /// Clears dictionary from not used semaphores to cleanup memory.
    /// </summary>
    private void CleanupInactiveUsers()
    {
        lock (_cleanupLock)
        {
            if (_userLocks.Count <= _maxTrackedUsers)
                return;

            int removed = 0;
            var now = DateTime.UtcNow;
            foreach (var (key, (sem, last)) in _userLocks)
            {
                if (removed >= _userLocks.Count / 4) // Clear less than 25% for performance
                    break;

                if ((now - last) > _lockExpiration && sem.CurrentCount == 1 && _userLocks.TryRemove(key, out _))
                {
                    sem.Dispose();
                    removed++;
                }
            }
        }
    }

    /// <summary>
    /// Clean up pipeline resources.
    /// </summary>
    public void Dispose()
    {
        foreach (var (_, (sem, _)) in _userLocks)
            sem.Dispose();
        _userLocks.Clear();
    }
}
