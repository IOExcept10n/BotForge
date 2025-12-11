using System.Reflection;
using BotForge.Fsm;
using BotForge.Hosting;
using BotForge.Localization;
using BotForge.Modules.Roles;
using BotForge.Persistence.Repositories;
using BotForge.Persistence.Roles;
using BotForge.Persistence.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BotForge.Persistence;

/// <summary>
/// Provides extension methods for configuring BotForge persistence services.
/// </summary>
public static class ServiceExtensions
{
    /// <param name="services">The service collection to configure.</param>
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds persistence support services with default SQLite configuration.
        /// This method registers the database context, repositories, and replaces in-memory services with persistent implementations.
        /// </summary>
        /// <returns>The updated service collection.</returns>
        public IServiceCollection AddPersistenceServices()
        {
            return services.AddPersistenceServices(options => { });
        }

        /// <summary>
        /// Adds persistence support services with the specified configuration.
        /// </summary>
        /// <param name="configure">Action to configure persistence options.</param>
        /// <returns>The updated service collection.</returns>
        public IServiceCollection AddPersistenceServices(Action<IPersistenceBuilder> configure)
        {
            ArgumentNullException.ThrowIfNull(configure);

            var options = new PersistenceOptions();
            var builder = new PersistenceBuilder(options);
            configure(builder);

            return services.AddPersistenceCore<BotForgeDbContext>(options);
        }

        /// <summary>
        /// Adds persistence support services with a custom database context type with default SQLite configuration.
        /// </summary>
        /// <typeparam name="TContext">The type of the database context, which must inherit from <see cref="BotForgeDbContext"/>.</typeparam>
        /// <returns>The updated service collection.</returns>
        public IServiceCollection AddPersistenceServices<TContext>()
            where TContext : BotForgeDbContext
        {
            var contextAssembly = typeof(TContext).Assembly;
            return services.AddPersistenceServices<TContext>(x => x.UseMigrationsAssembly(contextAssembly).ScanRepositoriesFrom(contextAssembly));
        }

        /// <summary>
        /// Adds persistence support services with a custom database context type.
        /// </summary>
        /// <remarks>
        /// Don't forget to configure assemblies for repositories and migrations.
        /// </remarks>
        /// <typeparam name="TContext">The type of the database context, which must inherit from <see cref="BotForgeDbContext"/>.</typeparam>
        /// <param name="configure">Action to configure persistence options.</param>
        /// <returns>The updated service collection.</returns>
        public IServiceCollection AddPersistenceServices<TContext>(Action<IPersistenceBuilder> configure)
            where TContext : BotForgeDbContext
        {
            ArgumentNullException.ThrowIfNull(configure);

            var options = new PersistenceOptions();
            var builder = new PersistenceBuilder(options);
            configure(builder);

            return services.AddPersistenceCore<TContext>(options);
        }

        private IServiceCollection AddPersistenceCore<TContext>(PersistenceOptions options)
            where TContext : BotForgeDbContext
        {
            return services.AddPersistenceCore<TContext>((sp, builder) =>
            {
                ConfigureDbContextOptions(sp, builder, options);
            }, options);
        }

        private IServiceCollection AddPersistenceCore<TContext>(Action<IServiceProvider, DbContextOptionsBuilder> configureDbContext, PersistenceOptions options)
            where TContext : BotForgeDbContext
        {
            // Register DbContext
            services.AddDbContext<TContext>(configureDbContext);

            return services.AddPersistenceServices<TContext>(options);
        }

        private IServiceCollection AddPersistenceServices<TContext>(PersistenceOptions options)
            where TContext : BotForgeDbContext
        {
            // Register default repositories
            services.TryAddScoped<IBotUserRepository, BotUserRepository>();
            services.TryAddScoped<IBotRoleRepository, BotRoleRepository>();

            // Auto-register repositories if enabled
            if (options.RegisterRepositoriesAutomatically)
            {
                services.RegisterRepositories(options.RepositoriesAssembly ?? typeof(BotForgeDbContext).Assembly);
            }

            // Replace in-memory services with persistent implementations
            if (options.ReplaceUserStateStore)
            {
                services.Replace(ServiceDescriptor.Singleton<IUserStateStore, PersistentUserStateStore>());
            }

            if (options.ReplaceUserLocaleProvider)
            {
                services.Replace(ServiceDescriptor.Singleton<IUserLocaleProvider, PersistentUserLocaleProvider>());
            }

            if (options.ReplaceRoleStorage)
            {
                services.TryAddScoped<PersistentRoleStorage>();
                services.Replace(ServiceDescriptor.Scoped<IRoleProvider>(sp => sp.GetRequiredService<PersistentRoleStorage>()));
                services.Replace(ServiceDescriptor.Scoped<IRoleManager>(sp => sp.GetRequiredService<PersistentRoleStorage>()));
            }

            // Register migration hosted service if needed
            if (options.AutoMigrate || options.UseEnsureCreated)
            {
                services.AddSingleton(options);
                services.AddHostedService<MigrationHostedService>();
            }

            // Register roles seed service
            services.AddHostedService<RolesSeedHostedService>();

            return services;
        }

        private void RegisterRepositories(Assembly assembly)
        {
            var repositoryTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericTypeDefinition)
                .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRepository<,>)))
                .ToList();

            foreach (var repositoryType in repositoryTypes)
            {
                // Skip default repositories (already registered)
                if (repositoryType == typeof(BotUserRepository) || repositoryType == typeof(BotRoleRepository))
                    continue;

                // Register as scoped
                services.TryAddScoped(repositoryType);

                // Register all interfaces implemented by the repository
                var interfaces = repositoryType.GetInterfaces()
                    .Where(i => i != typeof(IDisposable) && i != typeof(IAsyncDisposable))
                    .ToList();

                foreach (var interfaceType in interfaces)
                {
                    services.TryAddScoped(interfaceType, repositoryType);
                }

                // Register as IRepository<TKey, TEntity> if applicable
                var repositoryInterface = repositoryType.GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRepository<,>));

                if (repositoryInterface != null)
                {
                    var genericArgs = repositoryInterface.GetGenericArguments();
                    if (genericArgs.Length == 2)
                    {
                        var keyType = genericArgs[0];
                        var entityType = genericArgs[1];

                        // Register as IRepository<TKey, TEntity>
                        var keyedRepositoryType = typeof(IRepository<,>).MakeGenericType(keyType, entityType);
                        services.TryAddScoped(keyedRepositoryType, repositoryType);

                        // Register as IRepository<TEntity>
                        var entityRepositoryType = typeof(IRepository<>).MakeGenericType(entityType);
                        services.TryAddScoped(entityRepositoryType, repositoryType);
                    }
                }
            }
        }
    }

    private static void ConfigureDbContextOptions(IServiceProvider serviceProvider, DbContextOptionsBuilder builder, PersistenceOptions options)
    {
        // Determine migrations assembly
        var migrationsAssembly = options.MigrationsAssembly ?? typeof(BotForgeDbContext).Assembly;
        var migrationsAssemblyName = migrationsAssembly.GetName().Name;
        
        // Use custom configuration if provided
        if (options.ConfigureDbContext != null)
        {
            // Apply custom configuration first
            options.ConfigureDbContext(builder);
            
            // Check if migrations assembly was set by custom configuration
            var extension = builder.Options.Extensions
                .OfType<Microsoft.EntityFrameworkCore.Infrastructure.RelationalOptionsExtension>()
                .FirstOrDefault();
            return;
        }

        // Try to get connection string from configuration
        string? connectionString = options.ConnectionString;
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            var configuration = serviceProvider.GetService<IConfiguration>();
            connectionString = configuration?["ConnectionStrings:BotForge"] ?? configuration?["ConnectionStrings:DefaultConnection"];
        }

        // Default to SQLite if no connection string is provided
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = "Data Source=botforge.db";
        }

        // Configure SQLite with migrations assembly
        builder.UseSqlite(connectionString, sqliteOptions => 
        {
            sqliteOptions.MigrationsAssembly(migrationsAssembly);
        });
    }
}
