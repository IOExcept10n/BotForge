using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace BotForge.Persistence;

internal class PersistenceBuilder : IPersistenceBuilder
{
    private readonly PersistenceOptions _options;

    public PersistenceBuilder(PersistenceOptions options)
    {
        _options = options;
    }

    public IPersistenceBuilder ConfigureDbContext(Action<DbContextOptionsBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        _options.ConfigureDbContext = configure;
        return this;
    }

    public IPersistenceBuilder UseConnectionString(string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        _options.ConnectionString = connectionString;
        return this;
    }

    public IPersistenceBuilder ReplaceInMemoryServices(bool replace = true)
    {
        _options.ReplaceUserStateStore = replace;
        _options.ReplaceUserLocaleProvider = replace;
        _options.ReplaceRoleStorage = replace;
        return this;
    }

    public IPersistenceBuilder ReplaceUserStateStore(bool replace = true)
    {
        _options.ReplaceUserStateStore = replace;
        return this;
    }

    public IPersistenceBuilder ReplaceUserLocaleProvider(bool replace = true)
    {
        _options.ReplaceUserLocaleProvider = replace;
        return this;
    }

    public IPersistenceBuilder ReplaceRoleStorage(bool replace = true)
    {
        _options.ReplaceRoleStorage = replace;
        return this;
    }

    public IPersistenceBuilder AutoMigrate(bool autoMigrate = true)
    {
        _options.AutoMigrate = autoMigrate;
        return this;
    }

    public IPersistenceBuilder UseEnsureCreated(bool useEnsureCreated = true)
    {
        _options.UseEnsureCreated = useEnsureCreated;
        if (useEnsureCreated)
            _options.AutoMigrate = false;
        return this;
    }

    public IPersistenceBuilder AutoRegisterRepositories(bool enabled = true)
    {
        _options.RegisterRepositoriesAutomatically = enabled;
        return this;
    }

    public IPersistenceBuilder ScanRepositoriesFrom(Assembly? assembly)
    {
        _options.RepositoriesAssembly = assembly;
        return this;
    }

    public IPersistenceBuilder UseMigrationsAssembly(Assembly? assembly)
    {
        _options.MigrationsAssembly = assembly;
        return this;
    }
}

