using BotForge.Modules;
using BotForge.Modules.Roles;

namespace BotForge.Hosting;

/// <summary>
/// Contains some optional extensions for bot app host to simplify configuration.
/// </summary>
public static class HostExtensions
{
    /// <param name="builder">An instance of the bot app host builder to configure.</param>
    extension(IBotBuilder builder)
    {
        /// <summary>
        /// Removes main menu with modules selection by binding start command with the single module root directly.
        /// </summary>
        /// <remarks>
        /// This method simplifies bot menu in case your app has a single module.
        ///
        /// To use this method you should ensure that you have only one module in your application, or the initialization process will throw an exception.
        /// </remarks>
        /// <returns>The updated bot app builder instance.</returns>
        public IBotBuilder SkipModuleSelection()
        {
            builder.Services.ConfigureSingleModuleMenu();
            return builder;
        }

        /// <summary>
        /// Adds a welcome message in case bot don't use roles system for users.
        /// </summary>
        /// <remarks>
        /// Note that you shouldn't use this method when you configure roles manually.
        /// </remarks>
        /// <param name="welcomeMessageKey">A welcome message translation key for all bot users.</param>
        /// <returns>The updated bot app builder instance.</returns>
        public IBotBuilder UseWelcomeMessage(string welcomeMessageKey)
        {
            builder.Services.ConfigureRoles(b => b.AddRole<UnknownRole>(welcomeMessageKey));
            return builder;
        }
    }
}
