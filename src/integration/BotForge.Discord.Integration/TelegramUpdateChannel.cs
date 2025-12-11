using BotForge.Messaging;
using BotForge.Telegram.Integration.Models;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace BotForge.Telegram.Integration;

internal class TelegramUpdateChannel : IUpdateChannel, IUpdateHandler
{
    public event EventHandler<UpdateEventArgs> OnUpdate = delegate { };

    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
    {
        OnUpdate(botClient, new(new TelegramUpdate(DateTimeOffset.UtcNow, UpdateType.System, new(0), null, null, exception)));
        return Task.CompletedTask;
    }

    public Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        OnUpdate(botClient, new(update.ToBotForge()));
        return Task.CompletedTask;
    }
}
