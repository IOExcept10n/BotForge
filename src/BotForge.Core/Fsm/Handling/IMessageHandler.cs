using BotForge.Messaging;

namespace BotForge.Fsm.Handling;

/// <summary>
/// Receives message updates and dispatches them into the FSM or handler pipeline.
/// </summary>
public interface IMessageHandler
{
    /// <summary>
    /// Handles an incoming message update.
    /// </summary>
    /// <param name="message">The incoming message to process. Cannot be null.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous handling operation.</returns>
    Task HandleMessageAsync(IMessage message, CancellationToken cancellationToken);
}
