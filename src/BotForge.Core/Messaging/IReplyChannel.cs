namespace BotForge.Core.Messaging;

public interface IReplyChannel
{
    Task SendAsync(UserIdentity user, ReplyContext ctx, CancellationToken ct = default);

    Task RemoveKeyboardAsync(UserIdentity user, CancellationToken ct = default);

    Task DeleteLastUserMessageAsync(UserIdentity user, CancellationToken ct = default);

    Task DeleteLastReplyAsync(UserIdentity user, CancellationToken ct = default);
}
