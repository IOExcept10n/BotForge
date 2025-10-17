using BotForge.Messaging;

namespace BotForge.Fsm.Handling;

/// <summary>
/// Receives resolved interaction updates (for example, button presses or command invocations) and dispatches them
/// to the appropriate command/state logic.
/// </summary>
public interface IInteractionHandler
{
    /// <summary>
    /// Handles an interaction update.
    /// </summary>
    /// <param name="interaction">The interaction to process. Cannot be null.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous handling operation.</returns>
    Task HandleInteractionAsync(IInteraction interaction, CancellationToken cancellationToken);
}
