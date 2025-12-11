using System.Diagnostics;
using BotForge.Messaging;

namespace BotForge.Telegram.Integration.Models;

/// <summary>
/// DTO wrapper implementing <see cref="IMessage"/>. Constructed by <see cref="TelegramUpdateParser"/>.
/// </summary>
/// <inheritdoc/>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class TelegramMessage(UserIdentity from, ChatId chatId, MessageContent content) : IMessage
{
    /// <inheritdoc/>
    public UserIdentity From { get; } = from;

    /// <inheritdoc/>
    public ChatId ChatId { get; } = chatId;

    /// <inheritdoc/>
    public MessageContent Content { get; } = content;

    private string GetDebuggerDisplay()
    {
        string messageContent = Content switch
        {
            TextMessageContent text => text.Text,
            FileMessageContent file => $"File #{file.FileId}",
            _ => Content.ToString()
        };
        return $"[@{From.Username}]: {messageContent}";
    }
}
