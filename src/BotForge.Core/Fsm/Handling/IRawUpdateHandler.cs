using BotForge.Messaging;

namespace BotForge.Fsm.Handling;

/// <summary>
/// A fallback handler for raw updates that are not otherwise handled by message or interaction handlers.
/// Implementations may be used to log, inspect, or provide default handling for unknown update shapes.
/// </summary>
public interface IRawUpdateHandler
{
    /// <summary>
    /// Handles a raw update.
    /// </summary>
    /// <param name="update">The raw update to process. Cannot be null.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous handling operation.</returns>
    Task HandleAsync(IUpdate update, CancellationToken cancellationToken);
}
