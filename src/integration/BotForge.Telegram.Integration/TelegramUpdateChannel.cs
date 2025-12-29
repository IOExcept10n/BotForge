using BotForge.Localization;
using BotForge.Messaging;
using BotForge.Telegram.Integration.Models;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace BotForge.Telegram.Integration;

internal class TelegramUpdateChannel(IUserLocaleProvider localeProvider) : IUpdateChannel, IUpdateHandler
{
    private readonly IUserLocaleProvider _localeProvider = localeProvider;

    public event EventHandler<UpdateEventArgs> OnUpdate = delegate { };

    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
    {
        OnUpdate(botClient, new(new TelegramUpdate(DateTimeOffset.UtcNow, UpdateType.System, new(0), null, null, exception)));
        return Task.CompletedTask;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        => OnUpdate(botClient, new(await update.ToBotForgeAsync(_localeProvider, cancellationToken).ConfigureAwait(false)));
}
