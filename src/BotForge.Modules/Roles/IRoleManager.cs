using BotForge.Messaging;

namespace BotForge.Modules.Roles;

/// <summary>
/// Defines a manager for handling user roles within the BotForge framework.
/// </summary>
public interface IRoleManager
{
    /// <summary>
    /// Asynchronously sets the specified role for a user identified by their user ID.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose role is to be set.</param>
    /// <param name="role">The <see cref="Role"/> to assign to the user.</param>
    /// <param name="cancellationToken">A token that allows the operation to be canceled.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SetRoleByUserIdAsync(long userId, Role role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously sets the specified role for a user identified by their username and optional discriminator.
    /// </summary>
    /// <param name="username">The username of the user whose role is to be set.</param>
    /// <param name="discriminator">An optional discriminator for the user, commonly used to differentiate users with the same username.</param>
    /// <param name="role">The <see cref="Role"/> to assign to the user.</param>
    /// <param name="cancellationToken">A token that allows the operation to be canceled.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SetRoleByUsernameAsync(string username, string? discriminator, Role role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously sets the specified role for a user identified by their user identity.
    /// </summary>
    /// <param name="user">An instance of <see cref="UserIdentity"/> representing the user whose role is to be set.</param>
    /// <param name="role">The <see cref="Role"/> to assign to the user.</param>
    /// <param name="cancellationToken">A token that allows the operation to be canceled.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SetRoleByUserIdentityAsync(UserIdentity user, Role role, CancellationToken cancellationToken = default);
}
