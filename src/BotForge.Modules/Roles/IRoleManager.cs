using BotForge.Messaging;

namespace BotForge.Modules.Roles;

public interface IRoleManager
{
    Task SetRoleByUserIdAsync(long userId, Role role, CancellationToken cancellationToken = default);

    Task SetRoleByUsernameAsync(string username, string? discriminator, Role role, CancellationToken cancellationToken = default);

    Task SetRoleByUserIdentityAsync(UserIdentity user, Role role, CancellationToken cancellationToken = default);
}
