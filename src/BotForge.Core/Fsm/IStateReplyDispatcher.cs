namespace BotForge.Fsm;

/// <summary>
/// Dispatches replies produced by state execution to the appropriate channels.
/// </summary>
public interface IStateReplyDispatcher
{
    /// <summary>
    /// Sends the output described by <paramref name="result"/> using information from <paramref name="ctx"/>.
    /// Implementations should handle transitions, messages, or other reply actions indicated by the <see cref="StateResult"/>.
    /// </summary>
    /// <param name="result">The <see cref="StateResult"/> produced by a state handler indicating what to send or what transition to perform.</param>
    /// <param name="ctx">The <see cref="StateContext"/> containing contextual information such as user, chat, and services.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous dispatch operation.</returns>
    Task SendAsync(StateResult result, StateContext ctx, CancellationToken cancellationToken = default);
}
