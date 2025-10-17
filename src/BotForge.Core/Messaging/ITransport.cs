namespace BotForge.Messaging;

/// <summary>
/// Represents a transport implementation that integrates a specific messaging platform
/// with the FSM pipeline.
/// </summary>
public interface ITransport
{
    /// <summary>
    /// The underlying transport client (for example a platform SDK client). Transport-specific.
    /// </summary>
    object RawClient { get; }

    /// <summary>
    /// Channel used to send replies back to users.
    /// </summary>
    IReplyChannel ReplyChannel { get; }

    /// <summary>
    /// Channel that provides incoming updates to the application.
    /// </summary>
    IUpdateChannel UpdateChannel { get; }

    /// <summary>
    /// Starts the transport's background processing (for example subscribing to updates).
    /// </summary>
    Task StartAsync(CancellationToken ct = default);
}
