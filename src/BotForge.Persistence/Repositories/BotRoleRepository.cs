using Microsoft.EntityFrameworkCore;
using BotForge.Modules.Roles;
using BotForge.Persistence.Models;

namespace BotForge.Persistence.Repositories;

internal sealed class BotRoleRepository(BotForgeDbContext context) : Repository<BotForgeDbContext, long, BotRole>(context), IBotRoleRepository
{
    public async Task<BotRole> RegisterAsync(Role role, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(role);

        var existing = await DbSet.FirstOrDefaultAsync(r => r.Name == role.Name, cancellationToken).ConfigureAwait(false);
        if (existing != null)
            return existing;

        var entity = new BotRole { Name = role.Name };
        await DbSet.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        await Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return entity;
    }

    public async Task<BotRole> GetRoleAsync(Role role, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(role);
        var existing = await DbSet.FirstOrDefaultAsync(r => r.Name == role.Name, cancellationToken).ConfigureAwait(false);
        if (existing == null)
            throw new InvalidOperationException($"Role '{role.Name}' is not registered in database.");
        return existing;
    }
}
