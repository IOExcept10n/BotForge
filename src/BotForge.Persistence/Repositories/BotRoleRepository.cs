using BotForge.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace BotForge.Persistence.Repositories;

public class BotRoleRepository(BotForgeDbContext context) : Repository<BotForgeDbContext, long, BotRole>(context), IBotRoleRepository
{
    public async Task<BotRole?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        => await Context.Roles.FirstOrDefaultAsync(r => r.Name == name, cancellationToken).ConfigureAwait(false);
}
