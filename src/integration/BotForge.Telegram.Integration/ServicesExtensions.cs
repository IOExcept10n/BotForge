using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BotForge.Telegram.Integration;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> class.
/// </summary>
public static class ServicesExtensions
{
    /// <param name="services">A collection of services to configure.</param>
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds telegram update and retry channels if they haven't been initialized yet.
        /// Also adds telegram transporting service that is ready for listening.
        /// </summary>
        /// <returns>The same instance of the <see cref="IServiceCollection"/> for fluent initialization.</returns>
        public IServiceCollection AddTelegramTransport()
        {
            services.TryAddSingleton<TelegramReplyChannel>();
            services.TryAddSingleton<TelegramUpdateChannel>();
            services.AddSingleton<TelegramTransport>();
            return services;
        }
    }
}
