namespace BotForge.Messaging;

/// <summary>
/// Channel that publishes incoming updates to subscribers.
/// </summary>
public interface IUpdateChannel
{
    /// <summary>
    /// Invoked when an update arrives. Handlers should complete quickly; the channel implementation
    /// is responsible for invocation semantics (sequential vs parallel).
    /// </summary>
    event EventHandler<UpdateEventArgs> OnUpdate;
}

/// <summary>
/// Represents <see cref="EventArgs"/> for <see cref="IUpdateChannel"/> updates.
/// </summary>
/// <param name="update">An update to use.</param>
public class UpdateEventArgs(IUpdate update) : EventArgs
{
    /// <summary>
    /// Gets an update related to event.
    /// </summary>
    public IUpdate Update { get; init; } = update;
}
