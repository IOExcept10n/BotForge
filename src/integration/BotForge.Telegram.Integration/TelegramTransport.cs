using BotForge.Messaging;
using CommunityToolkit.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace BotForge.Telegram.Integration;

internal class TelegramTransport(ITelegramBotClient client, IReplyChannel replyChannel, IUpdateChannel updateChannel) : ITransport
{
    public object RawClient { get; } = client;

    public IReplyChannel ReplyChannel { get; } = replyChannel;

    public IUpdateChannel UpdateChannel { get; } = updateChannel;

    public async Task StartAsync(CancellationToken ct = default)
    {
        if (UpdateChannel is not IUpdateHandler updateHandler)
        {
            ThrowHelper.ThrowInvalidOperationException("Couldn't target updates listening to the selected update handler.");
            return;
        }
        client.StartReceiving(updateHandler, cancellationToken: ct);
    }
}
