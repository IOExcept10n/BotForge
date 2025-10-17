namespace BotForge.Fsm;

/// <summary>
/// Handles execution of a state within the finite-state machine.
/// Implementations contain the logic to process a state's step given the current message context
/// and produce a <see cref="StateResult"/> describing the next action (transition, repeat, or finish).
/// </summary>
public interface IStateHandler
{
    /// <summary>
    /// Executes the state logic using the provided <paramref name="ctx"/>.
    /// </summary>
    /// <param name="ctx">The <see cref="MessageStateContext"/> containing user state, message data, and services required by the state handler. Cannot be null.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> that completes with a <see cref="StateResult"/> indicating the outcome:
    /// for example, the next state to transition to, whether to repeat the current state, or to finish the conversation.
    /// </returns>
    Task<StateResult> ExecuteAsync(MessageStateContext ctx, CancellationToken cancellationToken = default);
}
