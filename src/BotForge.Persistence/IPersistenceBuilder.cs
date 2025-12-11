using Microsoft.EntityFrameworkCore;

namespace BotForge.Persistence;

/// <summary>
/// Provides a fluent interface for configuring BotForge persistence services.
/// </summary>
public interface IPersistenceBuilder
{
    /// <summary>
    /// Configures the database context options using the provided action.
    /// This takes precedence over connection string configuration.
    /// </summary>
    /// <param name="configure">Action to configure the DbContextOptionsBuilder.</param>
    /// <returns>The same builder instance for fluent chaining.</returns>
    IPersistenceBuilder ConfigureDbContext(Action<DbContextOptionsBuilder> configure);

    /// <summary>
    /// Sets the connection string for the database.
    /// This is ignored if <see cref="ConfigureDbContext"/> is used.
    /// </summary>
    /// <param name="connectionString">The database connection string.</param>
    /// <returns>The same builder instance for fluent chaining.</returns>
    IPersistenceBuilder UseConnectionString(string connectionString);

    /// <summary>
    /// Configures whether to automatically replace in-memory services with persistent implementations.
    /// </summary>
    /// <param name="replace">True to replace services automatically (default), false to skip.</param>
    /// <returns>The same builder instance for fluent chaining.</returns>
    IPersistenceBuilder ReplaceInMemoryServices(bool replace = true);

    /// <summary>
    /// Configures whether to replace the <see cref="BotForge.Fsm.IUserStateStore"/> service.
    /// </summary>
    /// <param name="replace">True to replace (default), false to skip.</param>
    /// <returns>The same builder instance for fluent chaining.</returns>
    IPersistenceBuilder ReplaceUserStateStore(bool replace = true);

    /// <summary>
    /// Configures whether to replace the <see cref="BotForge.Localization.IUserLocaleProvider"/> service.
    /// </summary>
    /// <param name="replace">True to replace (default), false to skip.</param>
    /// <returns>The same builder instance for fluent chaining.</returns>
    IPersistenceBuilder ReplaceUserLocaleProvider(bool replace = true);

    /// <summary>
    /// Configures whether to replace role storage services (<see cref="BotForge.Modules.Roles.IRoleProvider"/> and <see cref="BotForge.Modules.Roles.IRoleManager"/>).
    /// </summary>
    /// <param name="replace">True to replace (default), false to skip.</param>
    /// <returns>The same builder instance for fluent chaining.</returns>
    IPersistenceBuilder ReplaceRoleStorage(bool replace = true);

    /// <summary>
    /// Configures automatic database migration behavior.
    /// </summary>
    /// <param name="autoMigrate">True to run migrations automatically on startup (default), false to disable.</param>
    /// <returns>The same builder instance for fluent chaining.</returns>
    IPersistenceBuilder AutoMigrate(bool autoMigrate = true);

    /// <summary>
    /// Configures whether to use EnsureCreated instead of migrations.
    /// </summary>
    /// <param name="useEnsureCreated">True to use EnsureCreated (only if AutoMigrate is false), false otherwise.</param>
    /// <returns>The same builder instance for fluent chaining.</returns>
    IPersistenceBuilder UseEnsureCreated(bool useEnsureCreated = true);

    /// <summary>
    /// Configures automatic repository discovery and registration.
    /// </summary>
    /// <param name="enabled">True to enable automatic discovery (default), false to disable.</param>
    /// <returns>The same builder instance for fluent chaining.</returns>
    IPersistenceBuilder AutoRegisterRepositories(bool enabled = true);

    /// <summary>
    /// Sets the assembly to scan for repository implementations.
    /// </summary>
    /// <param name="assembly">The assembly to scan. If null, uses the persistence assembly.</param>
    /// <returns>The same builder instance for fluent chaining.</returns>
    IPersistenceBuilder ScanRepositoriesFrom(System.Reflection.Assembly? assembly);

    /// <summary>
    /// Sets the assembly containing migrations. If not set, uses the BotForge.Persistence assembly (default migrations).
    /// </summary>
    /// <param name="assembly">The assembly containing migrations. If null, uses default migrations.</param>
    /// <returns>The same builder instance for fluent chaining.</returns>
    IPersistenceBuilder UseMigrationsAssembly(System.Reflection.Assembly? assembly);
}

