using BotForge.Messaging;

namespace BotForge.Fsm;

/// <summary>
/// Result returned by a state handler indicating the next state and any associated state data or override message.
/// </summary>
/// <param name="NextStateId">Identifier of the next state to transition to.</param>
/// <param name="NextStateData">Serialized state data to associate with the next state.</param>
/// <param name="OverrideNextStateMessage">
/// Optional <see cref="ReplyContext"/> used to override or augment the message sent when entering the next state.
/// </param>
public record StateResult(string NextStateId, string NextStateData, ReplyContext? OverrideNextStateMessage = null)
{
    /// <summary>
    /// Constructs a <see cref="StateResult"/> that transitions to <paramref name="nextStateId"/> with empty state data and no override message.
    /// </summary>
    /// <param name="nextStateId">Identifier of the next state.</param>
    public StateResult(string nextStateId) : this(nextStateId, string.Empty, null) { }
}
