using BotForge.Hosting;

namespace BotForge.Persistence;

/// <summary>
/// Contains persistence extensions for bot app host.
/// </summary>
public static class HostExtensions
{
    /// <param name="builder">An instance of the bot app host builder to configure.</param>
    extension(IBotBuilder builder)
    {
        /// <summary>
        /// Adds persistence support to BotForge app with default SQLite configuration.
        /// This method registers the database context, repositories, and replaces in-memory services with persistent implementations.
        /// </summary>
        /// <returns>The updated app builder instance.</returns>
        public IBotBuilder AddPersistence()
        {
            builder.Services.AddPersistenceServices();
            return builder; 
        }

        /// <summary>
        /// Adds persistence support to BotForge app with the specified configuration.
        /// </summary>
        /// <param name="configure">Action to configure persistence options.</param>
        /// <returns>The updated app builder instance.</returns>
        public IBotBuilder AddPersistence(Action<IPersistenceBuilder> configure)
        {
            builder.Services.AddPersistenceServices(configure);
            return builder;
        }

        /// <summary>
        /// Adds  persistence support to BotForge app with a custom database context type with default SQLite configuration.
        /// </summary>
        /// <typeparam name="TContext">The type of the database context, which must inherit from <see cref="BotForgeDbContext"/>.</typeparam>
        /// <returns>The updated service collection.</returns>
        public IBotBuilder AddPersistence<TContext>()
            where TContext : BotForgeDbContext
        {
            builder.Services.AddPersistenceServices<TContext>();
            return builder;
        }

        /// <summary>
        /// Adds persistence support services with a custom database context type. Don't forget to configure assemblies for repositories and migrations.
        /// </summary>
        /// <typeparam name="TContext">The type of the database context, which must inherit from <see cref="BotForgeDbContext"/>.</typeparam>
        /// <param name="configure">Action to configure persistence options.</param>
        /// <returns>The updated service collection.</returns>
        public IBotBuilder AddPersistence<TContext>(Action<IPersistenceBuilder> configure)
            where TContext : BotForgeDbContext
        {
            builder.Services.AddPersistenceServices<TContext>(configure);
            return builder;
        }
    }
}
