using BotForge.Messaging;

namespace BotForge.Telegram.Integration.Models;

/// <summary>
/// DTO wrapper implementing <see cref="IMessage"/>. Constructed by <see cref="TelegramUpdateParser"/>.
/// </summary>
/// <inheritdoc/>
public sealed class TelegramMessage(UserIdentity from, ChatId chatId, MessageContent content) : IMessage
{

    /// <inheritdoc/>
    public UserIdentity From { get; } = from;

    /// <inheritdoc/>
    public ChatId ChatId { get; } = chatId;

    /// <inheritdoc/>
    public MessageContent Content { get; } = content;
}
