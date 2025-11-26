using System.Linq;
using BotForge.Modules.Roles;
using BotForge.Messaging;
using BotForge.Persistence.Repositories;

namespace BotForge.Persistence.Services;

public class PersistentRoleProvider(IBotUserRepository _users, IBotRoleRepository _roles, IRoleCatalog _catalog) : IRoleProvider
{
    public async Task<Role> GetRoleAsync(UserIdentity user, CancellationToken cancellationToken = default)
    {
        var botUser = await _users.GetByPlatformIdAsync(user.Id, cancellationToken).ConfigureAwait(false);
        if (botUser is null)
            return _catalog.DefaultRole;

        if (botUser.RoleId == 0)
            return _catalog.DefaultRole;

        var dbRole = await _roles.GetByIdAsync(botUser.RoleId, cancellationToken).ConfigureAwait(false);
        if (dbRole is null)
            return _catalog.DefaultRole;

        var matched = _catalog.DefinedRoles.FirstOrDefault(r => r.Name == dbRole.Name);
        return matched ?? new Role(dbRole.Name);
    }
}
