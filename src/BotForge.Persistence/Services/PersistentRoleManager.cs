using BotForge.Modules.Roles;
using BotForge.Messaging;
using BotForge.Persistence.Models;
using BotForge.Persistence.Repositories;

namespace BotForge.Persistence.Services;

public class PersistentRoleManager(IBotUserRepository _users, IBotRoleRepository _roles, IRoleCatalog _catalog) : IRoleManager
{
    public async Task SetRoleByUserIdAsync(long userId, Role role)
    {
        var user = await _users.GetOrCreateByPlatformIdAsync(userId).ConfigureAwait(false);
        var dbRole = await _roles.GetByNameAsync(role.Name).ConfigureAwait(false);
        if (dbRole is null)
        {
            dbRole = new BotRole { Name = role.Name };
            await _roles.AddAsync(dbRole).ConfigureAwait(false);
            await _roles.SaveChangesAsync().ConfigureAwait(false);
        }

        user.RoleId = dbRole.Id;
        await _users.UpdateAsync(user).ConfigureAwait(false);
        await _users.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task SetRoleByUsernameAsync(string username, string? discriminator, Role role)
    {
        int disc = 0;
        if (!string.IsNullOrEmpty(discriminator) && int.TryParse(discriminator, out var d)) disc = d;
        var user = await _users.GetOrCreateByUsernameAsync(username, disc).ConfigureAwait(false);
        var dbRole = await _roles.GetByNameAsync(role.Name).ConfigureAwait(false);
        if (dbRole is null)
        {
            dbRole = new BotRole { Name = role.Name };
            await _roles.AddAsync(dbRole).ConfigureAwait(false);
            await _roles.SaveChangesAsync().ConfigureAwait(false);
        }

        user.RoleId = dbRole.Id;
        await _users.UpdateAsync(user).ConfigureAwait(false);
        await _users.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task SetRoleByUserIdentityAsync(UserIdentity user, Role role)
    {
        await SetRoleByUserIdAsync(user.Id, role).ConfigureAwait(false);
    }
}
