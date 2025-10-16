using BotForge.Messaging;

namespace BotForge.Fsm;

public interface IUserStateStore
{
    Task<StateRecord> GetUserStateAsync(UserIdentity user, CancellationToken ct = default);

    Task<StateRecord> GetUserRootStateAsync(UserIdentity user, CancellationToken ct = default);

    Task SaveAsync(UserIdentity user, StateResult result, CancellationToken ct = default);
}

public readonly record struct StateRecord(string Id, string StateData)
{
    public const string StartStateId = "start";

    public static readonly StateRecord StartState = new(StartStateId, string.Empty);
}
