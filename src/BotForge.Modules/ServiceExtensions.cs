using System.Reflection;
using BotForge.Modules.Roles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BotForge.Modules
{
    /// <summary>
    /// Provides extension methods for configuring chatbot modules and roles in the dependency injection container.
    /// </summary>
    public static class ServiceExtensions
    {
        /// <param name="services">The service collection to configure.</param>
        extension(IServiceCollection services)
        {
            /// <summary>
            /// Configures app modules using the specified builder and the configuration action.
            /// </summary>
            /// <typeparam name="TBuilder">The type of the module registry builder.</typeparam>
            /// <param name="builder">The module registry builder to configure.</param>
            /// <param name="configure">The action to configure the builder.</param>
            /// <returns>The updated service collection.</returns>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="configure"/> is null.</exception>
            public IServiceCollection ConfigureModules<TBuilder>(TBuilder builder, Action<TBuilder> configure) where TBuilder : IModuleRegistryBuilder
            {
                ArgumentNullException.ThrowIfNull(configure);
                configure(builder);
                services.AddSingleton(builder.Build());
                return services;
            }

            /// <summary>
            /// Configures app modules using the specified configuration action.
            /// </summary>
            /// <typeparam name="TBuilder">The type of the module registry builder.</typeparam>
            /// <param name="configure">The action to configure the builder.</param>
            /// <returns>The updated service collection.</returns>
            public IServiceCollection ConfigureModules<TBuilder>(Action<TBuilder> configure) where TBuilder : IModuleRegistryBuilder =>
                services.AddSingleton(s =>
                {
                    var builder = ActivatorUtilities.CreateInstance<TBuilder>(s);
                    configure(builder);
                    return builder.Build();
                });

            /// <summary>
            /// Configures app modules using the specified configuration action with a default builder type.
            /// </summary>
            /// <param name="configure">The action to configure the builder.</param>
            /// <returns>The updated service collection.</returns>
            public IServiceCollection ConfigureModules(Action<IModuleRegistryBuilder> configure) => services.ConfigureModules<ModuleRegistryBuilder>(configure);

            /// <summary>
            /// Adds fallback module configuration by using the entry assembly or the calling assembly.
            /// </summary>
            /// <returns>The updated service collection.</returns>
            public IServiceCollection AddFallbackModuleConfiguration()
            {
                services.TryAddSingleton(s =>
                {
                    var builder = ActivatorUtilities.CreateInstance<ModuleRegistryBuilder>(s);
                    builder.UseAssembly(Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly());
                    return builder.Build();
                });
                return services;
            }

            /// <summary>
            /// Configures roles using the specified builder and configuration action.
            /// </summary>
            /// <typeparam name="TBuilder">The type of the role catalog builder.</typeparam>
            /// <param name="builder">The role catalog builder to configure.</param>
            /// <param name="configure">The action to configure the builder.</param>
            /// <returns>The updated service collection.</returns>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="configure"/> is null.</exception>
            public IServiceCollection ConfigureRoles<TBuilder>(TBuilder builder, Action<TBuilder> configure) where TBuilder : IRoleCatalogBuilder
            {
                ArgumentNullException.ThrowIfNull(configure);
                configure(builder);
                services.AddSingleton(builder.Build());
                return services;
            }

            /// <summary>
            /// Configures roles using the specified configuration action.
            /// </summary>
            /// <typeparam name="TBuilder">The type of the role catalog builder.</typeparam>
            /// <param name="configure">The action to configure the builder.</param>
            /// <returns>The updated service collection.</returns>
            public IServiceCollection ConfigureRoles<TBuilder>(Action<TBuilder> configure) where TBuilder : IRoleCatalogBuilder
            {
                services.AddSingleton(s =>
                {
                    var builder = ActivatorUtilities.CreateInstance<TBuilder>(s);
                    configure(builder);
                    return builder.Build();
                });
                return services;
            }

            /// <summary>
            /// Configures roles using the specified configuration action with a default builder type.
            /// </summary>
            /// <param name="configure">The action to configure the builder.</param>
            /// <returns>The updated service collection.</returns>
            public IServiceCollection ConfigureRoles(Action<IRoleCatalogBuilder> configure) => services.ConfigureRoles<RoleCatalogBuilder>(configure);

            /// <summary>
            /// Adds fallback role configuration by using the unknown role as a default.
            /// </summary>
            /// <returns>The updated service collection.</returns>
            public IServiceCollection AddFallbackRoleConfiguration()
            {
                services.TryAddSingleton(s =>
                {
                    var builder = ActivatorUtilities.CreateInstance<RoleCatalogBuilder>(s);
                    builder.AddRole(Role.Unknown, "Welcome");
                    return builder.Build();
                });
                return services;
            }
        }
    }
}
