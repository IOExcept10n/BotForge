using System.Globalization;
using BotForge.Messaging;
using BotForge.Modules.Roles;
using BotForge.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

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

    public async Task SetRoleByUserIdAsync(long userId, Role role, CancellationToken cancellationToken = default)
    {
        var dbRole = await _roles.RegisterAsync(role, cancellationToken).ConfigureAwait(false);

        int attempts = 0;
        while (true)
        {
            var userIdentity = new UserIdentity(userId);
            var user = await _users.GetOrRegisterAsync(userIdentity, cancellationToken).ConfigureAwait(false);

            user.RoleId = dbRole.Id;
            try
            {
                await _users.UpdateAsync(user, cancellationToken).ConfigureAwait(false);
                await _users.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                return;
            }
            catch (DbUpdateConcurrencyException) when (attempts == 0)
            {
                attempts++;
                continue;
            }
        }
    }

    public async Task SetRoleByUserIdentityAsync(UserIdentity user, Role role, CancellationToken cancellationToken = default)
    {
        await SetRoleByUserIdAsync(user.Id, role, cancellationToken).ConfigureAwait(false);
    }

    public async Task SetRoleByUsernameAsync(string username, string? discriminator, Role role, CancellationToken cancellationToken = default)
    {
        int disc = 0;
        if (!string.IsNullOrWhiteSpace(discriminator))
            int.TryParse(discriminator, NumberStyles.Integer, CultureInfo.InvariantCulture, out disc);

        var userIdentity = new UserIdentity(0, username, null, disc);
        var dbRole = await _roles.RegisterAsync(role, cancellationToken).ConfigureAwait(false);

        int attempts = 0;
        while (true)
        {
            var user = await _users.GetOrRegisterAsync(userIdentity, cancellationToken).ConfigureAwait(false);
            user.RoleId = dbRole.Id;
            try
            {
                await _users.UpdateAsync(user, cancellationToken).ConfigureAwait(false);
                await _users.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                return;
            }
            catch (DbUpdateConcurrencyException) when (attempts == 0)
            {
                attempts++;
                continue;
            }
        }
    }
}
