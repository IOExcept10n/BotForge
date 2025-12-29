using BotForge.Hosting;
using BotForge.Telegram.Integration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

namespace BotForge.Telegram;

/// <summary>
/// Provides extension methods for configuring a bot builder with Telegram bot integration.
/// </summary>
public static class BotBuilderExtensions
{
    /// <param name="builder">The <see cref="IBotBuilder"/> instance to configure.</param>
    extension(IBotBuilder builder)
    {
        /// <summary>
        /// Configures the bot builder to use a Telegram bot client.
        /// </summary>
        /// <param name="token">Optional Telegram bot token. If null, the token is retrieved from the configuration.</param>
        /// <returns>The updated <see cref="IBotBuilder"/> instance for chaining.</returns>
        public IBotBuilder WithTelegramBot(string? token = null)
        {
            builder.Services.AddSingleton<ITelegramBotClient>(s =>
            {
                var config = s.GetRequiredService<IConfiguration>();
                token ??= config.GetSection("ApiKeys:Telegram").Value;
                ArgumentNullException.ThrowIfNull(token);
                var lifetime = s.GetRequiredService<IHostApplicationLifetime>();
                var cts = new CancellationTokenSource();
                lifetime.ApplicationStopping.Register(() => cts.Cancel());
                return new TelegramBotClient(token, cancellationToken: cts.Token);
            });
            builder.Services.AddTelegramTransport();
            return builder;
        }
    }
}
