using BotForge.Persistence.Models;

namespace BotForge.Persistence.Repositories;

public interface IBotUserRepository : IRepository<Guid, BotUser>
{
    Task<BotUser?> GetByPlatformIdAsync(long platformId, CancellationToken cancellationToken = default);

    Task<BotUser?> GetByUsernameAsync(string username, int discriminator = 0, CancellationToken cancellationToken = default);

    Task<BotUser> GetOrCreateByPlatformIdAsync(long platformId, CancellationToken cancellationToken = default);

    Task<BotUser> GetOrCreateByUsernameAsync(string username, int discriminator = 0, CancellationToken cancellationToken = default);
}
