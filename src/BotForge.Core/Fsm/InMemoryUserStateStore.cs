using System.Collections.Concurrent;
using BotForge.Core.Messaging;

namespace BotForge.Core.Fsm;

internal class InMemoryUserStateStore : IUserStateStore
{
    private readonly ConcurrentDictionary<UserIdentity, StateRecord> _stateStore = [];

    public Task<StateRecord> GetUserRootStateAsync(UserIdentity user, CancellationToken ct = default) => Task.FromResult(StateRecord.StartState);

    public Task<StateRecord> GetUserStateAsync(UserIdentity user, CancellationToken ct = default)
    {
        return Task.FromResult(_stateStore.GetOrAdd(user, StateRecord.StartState));
    }

    public Task SaveAsync(UserIdentity user, StateResult result, CancellationToken ct = default)
    {
        StateRecord newState = new(result.NextStateId, result.NextStateData);
        _stateStore.AddOrUpdate(user, newState, (key, old) => newState);
        return Task.CompletedTask;
    }
}
