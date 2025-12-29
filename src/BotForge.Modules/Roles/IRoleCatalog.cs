namespace BotForge.Modules.Roles;

/// <summary>
/// Defines a catalog for managing and retrieving roles within the BotForge framework.
/// </summary>
public interface IRoleCatalog
{
    /// <summary>
    /// Gets an enumerable collection of all defined roles in the system.
    /// </summary>
    IEnumerable<Role> DefinedRoles { get; }

    /// <summary>
    /// Gets the default role assigned to users who do not have a specific role.
    /// </summary>
    Role DefaultRole { get; }

    /// <summary>
    /// Lists all available modules that a specified role can access.
    /// </summary>
    /// <param name="role">The <see cref="Role"/> for which to list available modules.</param>
    /// <returns>
    /// A read-only collection of <see cref="ModuleDescriptor"/> that represents the modules accessible to the specified role.
    /// </returns>
    IReadOnlyCollection<ModuleDescriptor> ListAvailableModules(Role role);

    /// <summary>
    /// Retrieves a welcome message tailored for a specified role.
    /// </summary>
    /// <param name="role">The <see cref="Role"/> for which to get the welcome message.</param>
    /// <returns>A string containing the welcome message for the specified role.</returns>
    string GetWelcomeMessage(Role role);
}
