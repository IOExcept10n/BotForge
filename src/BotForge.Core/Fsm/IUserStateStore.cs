using BotForge.Messaging;

namespace BotForge.Fsm;

/// <summary>
/// Represents abstraction for persisting and retrieving a user's current state in the FSM.
/// </summary>
public interface IUserStateStore
{
    /// <summary>
    /// Retrieves the current state record for the specified <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user identity whose state to retrieve.</param>
    /// <param name="ct">A token to monitor for cancellation requests.</param>
    /// <returns>A task that completes with the user's <see cref="StateRecord"/>. If no state exists, implementations may return <see cref="StateRecord.StartState"/>.</returns>
    Task<StateRecord> GetUserStateAsync(UserIdentity user, CancellationToken ct = default);

    /// <summary>
    /// Retrieves the user's root state record (for example, the top-level conversation or flow start).
    /// </summary>
    /// <param name="user">The user identity whose root state to retrieve.</param>
    /// <param name="ct">A token to monitor for cancellation requests.</param>
    /// <returns>A task that completes with the user's root <see cref="StateRecord"/>.</returns>
    Task<StateRecord> GetUserRootStateAsync(UserIdentity user, CancellationToken ct = default);

    /// <summary>
    /// Persists the provided <paramref name="result"/> as the user's new state.
    /// </summary>
    /// <param name="user">The user identity whose state will be saved.</param>
    /// <param name="result">The resulting state produced by a state handler that should be persisted.</param>
    /// <param name="ct">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    Task SaveAsync(UserIdentity user, StateResult result, CancellationToken ct = default);
}

/// <summary>
/// Represents the persisted state record for a user.
/// </summary>
/// <param name="Id">The state identifier.</param>
/// <param name="StateData">Serialized state data associated with the state.</param>
public readonly record struct StateRecord(string Id, string StateData)
{
    /// <summary>
    /// Identifier for the start state.
    /// </summary>
    public const string StartStateId = "start";

    /// <summary>
    /// Predefined <see cref="StateRecord"/> representing the start state with empty state data.
    /// </summary>
    public static readonly StateRecord StartState = new(StartStateId, string.Empty);
}
