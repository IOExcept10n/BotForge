using System.Collections.Concurrent;
using System.Globalization;
using BotForge.Messaging;

namespace BotForge.Modules.Roles;

internal class InMemoryRoleStorage(IRoleCatalog roleCatalog) : IRoleProvider, IRoleManager
{
    private readonly ConcurrentDictionary<long, Role> _rolesMap = [];
    private readonly IRoleCatalog _roleCatalog = roleCatalog;
    private readonly ConcurrentDictionary<string, UserIdentity> _usersMapping = new(StringComparer.InvariantCultureIgnoreCase);
    private readonly ConcurrentDictionary<string, Role> _rolesMapByName = new(StringComparer.InvariantCultureIgnoreCase);

    public Task<Role> GetRoleAsync(UserIdentity user, CancellationToken cancellationToken = default)
    {
        EnsureMapped(user);
        return Task.FromResult(_rolesMap.GetOrAdd(user.Id, _roleCatalog.DefaultRole));
    }

    public Task SetRoleByUserIdAsync(long userId, Role role, CancellationToken cancellationToken = default)
    {
        _rolesMap[userId] = role;
        return Task.CompletedTask;
    }

    public Task SetRoleByUserIdentityAsync(UserIdentity user, Role role, CancellationToken cancellationToken = default)
    {
        EnsureMapped(user);
        _rolesMap[user.Id] = role;
        return Task.CompletedTask;
    }

    public Task SetRoleByUsernameAsync(string username, string? discriminator, Role role, CancellationToken cancellationToken = default)
    {
        if (_usersMapping.TryGetValue(username, out var user))
        {
            _rolesMap[user.Id] = role;
        }
        else
        {
            _rolesMapByName[username] = role;
        }
        return Task.CompletedTask;
    }

    private void EnsureMapped(UserIdentity user)
    {
        string searchName = user.Username ?? user.Id.ToString(CultureInfo.InvariantCulture);
        _usersMapping.TryAdd(searchName, user);
        if (_rolesMapByName.TryGetValue(searchName, out var role))
            _rolesMap[user.Id] = role;
    }
}
