using BotForge.Hosting;
using BotForge.Telegram.Integration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

namespace BotForge.Telegram;

public static class BotBuilderExtensions
{
    extension(IBotBuilder builder)
    {
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
