namespace BotForge.Core.Messaging;

public abstract record MessageContent
{
    public static UnknownMessageContent Unknown { get; } = new();
}
public sealed record UnknownMessageContent : MessageContent;
public record TextMessageContent(string Text) : MessageContent;
public record FileMessageContent(string FileId) : MessageContent;
