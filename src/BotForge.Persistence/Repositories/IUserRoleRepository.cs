using BotForge.Persistence.Models;

namespace BotForge.Persistence.Repositories;

public interface IUserRoleRepository : IRepository<long, UserRole>
{
    Task<UserRole?> GetByUserAsync(BotUser user, CancellationToken cancellationToken);

    Task SetByPlatformIdAsync(BotUser user, BotRole role, CancellationToken cancellationToken);
}
