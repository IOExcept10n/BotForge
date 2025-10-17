namespace BotForge.Messaging;

/// <summary>
/// Base type for message content variants (text, files, unknown, etc.).
/// </summary>
public abstract record MessageContent
{
    /// <summary>
    /// Represents unknown or unsupported content types.
    /// </summary>
    public static UnknownMessageContent Unknown { get; } = new();
}

/// <summary>
/// Represents an unknown message content type.
/// </summary>
public sealed record UnknownMessageContent : MessageContent;

/// <summary>
/// Represents textual message content.
/// </summary>
/// <param name="Text">The message text.</param>
public record TextMessageContent(string Text) : MessageContent;

/// <summary>
/// Represents a file-type message content identified by a transport-specific file id.
/// </summary>
/// <param name="FileId">Transport-specific file identifier.</param>
public record FileMessageContent(string FileId) : MessageContent;
