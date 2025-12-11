using BotForge.Messaging;
using BotForge.Persistence.Models;

namespace BotForge.Persistence.Repositories;

public interface IBotUserRepository : IRepository<Guid, BotUser>
{
    Task<BotUser?> GetByPlatformIdAsync(long platformId, CancellationToken cancellationToken = default);

    Task<BotUser?> GetByUsernameAsync(string username, int discriminator = 0, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an existing user or creates a new one, merging missing fields from the provided identity.
    /// </summary>
    /// <param name="user">The user identity to get or register.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The user entity, either existing or newly created.</returns>
    Task<BotUser> GetOrRegisterAsync(UserIdentity user, CancellationToken cancellationToken = default);
}
