using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using BotForge.Messaging;
using BotForge.Modules.Roles;
using BotForge.Persistence.Repositories;

namespace BotForge.Persistence.Services;

internal class PersistentRoleStorage(IBotRoleRepository roles, IBotUserRepository users, IRoleCatalog roleCatalog) : IRoleProvider, IRoleManager
{
    private readonly IBotRoleRepository _roles = roles;
    private readonly IBotUserRepository _users = users;
    private readonly IRoleCatalog _roleCatalog = roleCatalog;

    public async Task<Role> GetRoleAsync(UserIdentity user, CancellationToken cancellationToken = default)
    {
        var botUser = await _users.GetOrRegisterAsync(user, cancellationToken).ConfigureAwait(false);
        if (botUser.Role != null)
            return new Role(botUser.Role.Name);

        return _roleCatalog.DefaultRole;
    }

    public Task SetRoleByUserIdAsync(long userId, Role role)
    {
        return SetRoleByUserIdAsync(userId, role, CancellationToken.None);
    }

    public Task SetRoleByUserIdentityAsync(UserIdentity user, Role role)
    {
        return SetRoleByUserIdAsync(user.Id, role, CancellationToken.None);
    }

    public Task SetRoleByUsernameAsync(string username, string? discriminator, Role role)
    {
        return SetRoleByUsernameAsync(username, discriminator, role, CancellationToken.None);
    }

    private async Task SetRoleByUserIdAsync(long userId, Role role, CancellationToken ct)
    {
        var dbRole = await _roles.RegisterAsync(role, ct).ConfigureAwait(false);

        var userIdentity = new BotForge.Messaging.UserIdentity(userId);
        var user = await _users.GetOrRegisterAsync(userIdentity, ct).ConfigureAwait(false);

        user.RoleId = dbRole.Id;
        await _users.UpdateAsync(user, ct).ConfigureAwait(false);
        await _users.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    private async Task SetRoleByUsernameAsync(string username, string? discriminator, Role role, CancellationToken ct)
    {
        int disc = 0;
        if (!string.IsNullOrWhiteSpace(discriminator))
            int.TryParse(discriminator, NumberStyles.Integer, CultureInfo.InvariantCulture, out disc);

        var userIdentity = new BotForge.Messaging.UserIdentity(0, username, null, disc);
        var user = await _users.GetOrRegisterAsync(userIdentity, ct).ConfigureAwait(false);
        var dbRole = await _roles.RegisterAsync(role, ct).ConfigureAwait(false);

        user.RoleId = dbRole.Id;
        await _users.UpdateAsync(user, ct).ConfigureAwait(false);
        await _users.SaveChangesAsync(ct).ConfigureAwait(false);
    }
}
