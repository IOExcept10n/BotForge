using BotForge.Messaging;

namespace BotForge.Modules.Roles;

public interface IRoleManager
{
    Task SetRoleByUserIdAsync(long userId, Role role);

    Task SetRoleByUsernameAsync(string username, string? discriminator, Role role);

    Task SetRoleByUserIdentityAsync(UserIdentity user, Role role);
}
