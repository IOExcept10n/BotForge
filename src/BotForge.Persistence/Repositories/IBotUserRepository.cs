using BotForge.Messaging;
using BotForge.Persistence.Models;

namespace BotForge.Persistence.Repositories;

/// <summary>
/// Represents a repository interface for managing bot user entities.
/// </summary>
public interface IBotUserRepository : IRepository<Guid, BotUser>
{
    /// <summary>
    /// Asynchronously retrieves a bot user by their platform-specific identifier.
    /// </summary>
    /// <param name="platformId">The unique identifier for the user on a specific platform.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, containing the bot user object if found; otherwise, <see langword="null"/>.</returns>
    Task<BotUser?> GetByPlatformIdAsync(long platformId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves a bot user by their username and an optional discriminator.
    /// </summary>
    /// <param name="username">The username of the bot user.</param>
    /// <param name="discriminator">An optional discriminator to differentiate users with the same username.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, containing the bot user object if found; otherwise, <see langword="null"/>.</returns>
    Task<BotUser?> GetByUsernameAsync(string username, int discriminator = 0, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an existing user or creates a new one, merging missing fields from the provided identity.
    /// </summary>
    /// <param name="user">The user identity to get or register.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The user entity, either existing or newly created.</returns>
    Task<BotUser> GetOrRegisterAsync(UserIdentity user, CancellationToken cancellationToken = default);
}
