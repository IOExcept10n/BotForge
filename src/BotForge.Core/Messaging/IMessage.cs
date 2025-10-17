namespace BotForge.Messaging;

/// <summary>
/// Represents an incoming message with sender, chat identifier, and content.
/// </summary>
public interface IMessage
{
    /// <summary>
    /// The identity of the message sender.
    /// </summary>
    UserIdentity From { get; }

    /// <summary>
    /// The chat identifier where the message was sent.
    /// </summary>
    ChatId ChatId { get; }

    /// <summary>
    /// The message content (text, media, etc.).
    /// </summary>
    MessageContent Content { get; }
}

/// <summary>
/// Lightweight chat identifier wrapper.
/// </summary>
/// <param name="Id">Numeric chat identifier.</param>
public readonly record struct ChatId(long Id)
{
    /// <summary>
    /// Implicitly converts a <see cref="long"/> to <see cref="ChatId"/>.
    /// </summary>
    public static implicit operator ChatId(long id) => new(id);

    /// <summary>
    /// Creates a <see cref="ChatId"/> from a 64-bit integer.
    /// </summary>
    public static ChatId FromInt64(long id) => new(id);
}
