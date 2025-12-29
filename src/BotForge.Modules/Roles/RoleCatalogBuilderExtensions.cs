namespace BotForge.Modules.Roles;

/// <summary>
/// Provides extension methods for adding roles to an <see cref="IRoleCatalogBuilder"/>.
/// </summary>
public static class RoleCatalogBuilderExtensions
{
    /// <param name="builder">The <see cref="IRoleCatalogBuilder"/> instance to which the role will be added.</param>
    extension(IRoleCatalogBuilder builder)
    {
        /// <summary>
        /// Adds a new role to the role catalog using a specified welcome message key.
        /// </summary>
        /// <typeparam name="T">The type of the role to add, which must derive from <see cref="Role"/>.</typeparam>
        /// <param name="welcomeMessageKey">A string key representing the welcome message associated with the role.</param>
        /// <returns>The updated <see cref="IRoleCatalogBuilder"/> instance for chaining.</returns>
        public IRoleCatalogBuilder AddRole<T>(string welcomeMessageKey) where T : Role, new() => builder.AddRole(new T(), welcomeMessageKey);
    }
}
