using BotForge.Fsm;
using BotForge.Messaging;
using BotForge.Persistence.Models;
using BotForge.Persistence.Repositories;

namespace BotForge.Persistence.Services;

internal class PersistentUserStateStore(IBotUserRepository _users, IUserStateRepository _states) : IUserStateStore
{
    public Task<StateRecord> GetUserRootStateAsync(UserIdentity user, CancellationToken ct = default)
        => Task.FromResult(StateRecord.StartState);

    public async Task<StateRecord> GetUserStateAsync(UserIdentity user, CancellationToken ct = default)
    {
        var botUser = await _users.GetByPlatformIdAsync(user.Id, ct).ConfigureAwait(false);
        if (botUser is null)
            return StateRecord.StartState;

        var state = await _states.GetByUserIdAsync(botUser.Id, ct).ConfigureAwait(false);
        if (state is null)
            return StateRecord.StartState;

        string id = string.IsNullOrEmpty(state.StateId) ? StateRecord.StartStateId : state.StateId;
        string data = state.StateData ?? string.Empty;
        return new StateRecord(id, data);
    }

    public async Task SaveAsync(UserIdentity user, StateResult result, CancellationToken ct = default)
    {
        var botUser = await _users.GetOrCreateByPlatformIdAsync(user.Id, ct).ConfigureAwait(false);
        botUser.LastSeen = DateTimeOffset.UtcNow;
        await _users.UpdateAsync(botUser, ct).ConfigureAwait(false);

        var state = await _states.GetByUserIdAsync(botUser.Id, ct).ConfigureAwait(false);
        if (state is null)
        {
            state = new UserState
            {
                Id = Guid.NewGuid(),
                UserId = botUser.Id,
                StateId = result.NextStateId,
                StateData = result.NextStateData,
                UpdatedAt = DateTimeOffset.UtcNow
            };
            await _states.AddAsync(state, ct).ConfigureAwait(false);
        }
        else
        {
            state.StateId = result.NextStateId;
            state.StateData = result.NextStateData;
            state.UpdatedAt = DateTimeOffset.UtcNow;
            await _states.UpdateAsync(state, ct).ConfigureAwait(false);
        }

        await _users.SaveChangesAsync(ct).ConfigureAwait(false);
        await _states.SaveChangesAsync(ct).ConfigureAwait(false);
    }
}
