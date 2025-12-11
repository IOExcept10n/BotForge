namespace BotForge.Persistence;

using System.Reflection;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Preovides options for configuring persistene library.
/// </summary>
public sealed class PersistenceOptions
{
    /// <summary>
    /// Optional callback to configure the <see cref="DbContextOptionsBuilder"/>.
    /// If provided it takes precedence over <see cref="ConnectionString"/>.
    /// </summary>
    public Action<DbContextOptionsBuilder>? ConfigureDbContext { get; set; }

    /// <summary>
    /// Optional connection string used when no <see cref="ConfigureDbContext"/> is provided.
    /// Default: "Data Source=botforge.db" (SQLite).
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// If true, the hosted migration task will call <c>Database.MigrateAsync()</c> on startup.
    /// If false and <see cref="UseEnsureCreated"/> is true, <c>Database.EnsureCreated()</c> will be used instead.
    /// </summary>
    public bool AutoMigrate { get; set; } = true;

    /// <summary>
    /// If true and AutoMigrate is false, <c>EnsureCreated</c> will be called instead of migrations.
    /// </summary>
    public bool UseEnsureCreated { get; set; }

    /// <summary>
    /// If set, the assembly to scan for repository implementations to register automatically.
    /// Default is the assembly that contains the persistence types. It is always included for registration unless repositories registration is disabled manually.
    /// </summary>
    public Assembly? RepositoriesAssembly { get; set; }

    /// <summary>
    /// When true, repository types found in <see cref="RepositoriesAssembly"/> will be registered automatically.
    /// </summary>
    public bool RegisterRepositoriesAutomatically { get; set; } = true;

    /// <summary>
    /// When true, replaces <see cref="BotForge.Fsm.IUserStateStore"/> with persistent implementation.
    /// </summary>
    public bool ReplaceUserStateStore { get; set; } = true;

    /// <summary>
    /// When true, replaces <see cref="BotForge.Localization.IUserLocaleProvider"/> with persistent implementation.
    /// </summary>
    public bool ReplaceUserLocaleProvider { get; set; } = true;

    /// <summary>
    /// When true, replaces role storage services (<see cref="BotForge.Modules.Roles.IRoleProvider"/> and <see cref="BotForge.Modules.Roles.IRoleManager"/>) with persistent implementation.
    /// </summary>
    public bool ReplaceRoleStorage { get; set; } = true;

    /// <summary>
    /// Assembly containing migrations. If null, uses the BotForge.Persistence assembly (default migrations).
    /// Users can set this to their own assembly if they have custom migrations.
    /// </summary>
    public Assembly? MigrationsAssembly { get; set; }
}
