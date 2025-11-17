namespace BotForge.Messaging;

/// <summary>
/// Abstraction for sending replies and performing chat-level operations.
/// </summary>
public interface IReplyChannel
{
    /// <summary>
    /// Sends a reply to the specified user using the given <paramref name="ctx"/>.
    /// </summary>
    Task SendAsync(UserIdentity user, ReplyContext ctx, CancellationToken ct = default);

    /// <summary>
    /// Removes an inline keyboard or reply markup associated with the user's last message.
    /// </summary>
    Task RemoveKeyboardAsync(UserIdentity user, CancellationToken ct = default);
}
