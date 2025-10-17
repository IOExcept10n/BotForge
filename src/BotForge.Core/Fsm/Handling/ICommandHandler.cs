namespace BotForge.Fsm.Handling;

/// <summary>
/// Handles a chat command (for example, a slash command or button-invoked command).
/// </summary>
public interface ICommandHandler
{
    /// <summary>
    /// The name of the command this handler processes. Used to match incoming interactions to the handler.
    /// </summary>
    string CommandName { get; }

    /// <summary>
    /// Handles the command interaction and returns a <see cref="StateResult"/> describing the next state or reply.
    /// </summary>
    /// <param name="ctx">The interaction state context containing the interaction, current state, and services. Cannot be null.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that completes with a <see cref="StateResult"/> that indicates the next state and associated state data.
    /// </returns>
    Task<StateResult> HandleCommand(InteractionStateContext ctx, CancellationToken cancellationToken = default);
}
