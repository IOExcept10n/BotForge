namespace BotForge.Modules.Roles;

/// <summary>
/// Defines a configurator for setting up the main menu based on user roles.
/// </summary>
public interface IMainMenuConfigurator
{
    /// <summary>
    /// Adds a main menu entry for a specific role in the provided role catalog.
    /// </summary>
    /// <param name="catalog">An instance of <see cref="IRoleCatalog"/> that contains the roles available in the system.</param>
    /// <param name="role">The <see cref="Role"/> for which the main menu entry is being added.</param>
    void AddMainMenu(IRoleCatalog catalog, Role role);
}
