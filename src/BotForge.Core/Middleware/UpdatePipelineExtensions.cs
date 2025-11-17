using System.Runtime.CompilerServices;

namespace BotForge.Middleware;

/// <summary>
/// Extension methods to configure update pipeline options on <see cref="IUpdatePipelineBuilder"/>.
/// </summary>
public static class UpdatePipelineExtensions
{
    /// <param name="pipelineBuilder">The pipeline builder to configure. Cannot be <see langword="null"/>.</param>
    extension(IUpdatePipelineBuilder pipelineBuilder)
    {
        /// <summary>
        /// Sets the maximum number of tracked users for the update pipeline.
        /// </summary>
        /// <param name="maxUsers">Maximum number of users to track simultaneously. Must be greater than zero.</param>
        /// <returns>The same <see cref="IUpdatePipelineBuilder"/> instance for fluent chaining.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="maxUsers"/> is less than or equal to zero.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IUpdatePipelineBuilder WithMaxUsers(int maxUsers)
        {
            ArgumentNullException.ThrowIfNull(pipelineBuilder);
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(maxUsers, 0);
            pipelineBuilder.Config = pipelineBuilder.Config with { MaxTrackedUsers = maxUsers };
            return pipelineBuilder;
        }

        /// <summary>
        /// Sets the lock expiration duration used when locking user state during update processing.
        /// </summary>
        /// <param name="lockExpiration">Duration after which a lock expires. Must be > <see cref="TimeSpan.Zero"/>.</param>
        /// <returns>The same <see cref="IUpdatePipelineBuilder"/> instance for fluent chaining.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="lockExpiration"/> is less than or equal to <see cref="TimeSpan.Zero"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IUpdatePipelineBuilder WithLockExpiration(TimeSpan lockExpiration)
        {
            ArgumentNullException.ThrowIfNull(pipelineBuilder);
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(lockExpiration, TimeSpan.Zero);
            pipelineBuilder.Config = pipelineBuilder.Config with { LockExpiration = lockExpiration };
            return pipelineBuilder;
        }
    }
}
