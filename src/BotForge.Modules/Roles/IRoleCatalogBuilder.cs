namespace BotForge.Modules.Roles;

/// <summary>
/// Defines a builder for constructing a role catalog within the BotForge framework.
/// </summary>
public interface IRoleCatalogBuilder
{
    /// <summary>
    /// Sets the default role to be assigned to users who do not have a specific role.
    /// </summary>
    /// <param name="role">The <see cref="Role"/> to set as the default role.</param>
    /// <returns>The updated <see cref="IRoleCatalogBuilder"/> instance for chaining.</returns>
    IRoleCatalogBuilder SetDefaultRole(Role role);

    /// <summary>
    /// Adds a new role to the catalog with an associated welcome message key.
    /// </summary>
    /// <param name="role">The <see cref="Role"/> to add to the catalog.</param>
    /// <param name="welcomeMessageKey">A string key representing the welcome message to be associated with the role.</param>
    /// <returns>The updated <see cref="IRoleCatalogBuilder"/> instance for chaining.</returns>
    IRoleCatalogBuilder AddRole(Role role, string welcomeMessageKey);

    /// <summary>
    /// Builds and returns the constructed <see cref="IRoleCatalog"/>.
    /// </summary>
    /// <returns>An instance of <see cref="IRoleCatalog"/> representing the configured role catalog.</returns>
    IRoleCatalog Build();
}
