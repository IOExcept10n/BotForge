namespace BotForge.Core.Messaging;

public interface ITransport
{
    object RawClient { get; }

    IReplyChannel ReplyChannel { get; }

    IUpdateChannel UpdateChannel { get; }

    Task StartAsync(CancellationToken ct = default);
}
