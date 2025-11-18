using BotForge.Messaging;

namespace BotForge.Modules.Roles;

/// <summary>
/// Defines a contract for providing user roles.
/// </summary>
public interface IRoleProvider
{
    /// <summary>
    /// Asynchronously retrieves the role associated with a specified user.
    /// </summary>
    /// <param name="user">The data of the user for whom to retrieve the role.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation, if needed (optional).</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing the role associated with the user.
    /// </returns>
    Task<Role> GetRoleAsync(UserIdentity user, CancellationToken cancellationToken = default);
}
