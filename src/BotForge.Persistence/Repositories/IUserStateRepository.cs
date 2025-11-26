using BotForge.Persistence.Models;

namespace BotForge.Persistence.Repositories;

public interface IUserStateRepository : IRepository<Guid, UserState>
{
    Task<UserState?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
