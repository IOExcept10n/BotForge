using BotForge.Persistence.Models;

namespace BotForge.Persistence.Repositories;

public interface IBotRoleRepository : IRepository<long, BotRole>
{
    Task<BotRole?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}
