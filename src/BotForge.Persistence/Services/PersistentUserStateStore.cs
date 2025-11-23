using BotForge.Fsm;
using BotForge.Messaging;
using BotForge.Persistence.Models;

namespace BotForge.Persistence.Services;

internal class PersistentUserStateStore() : IUserStateStore
{
    public Task<StateRecord> GetUserRootStateAsync(UserIdentity user, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<StateRecord> GetUserStateAsync(UserIdentity user, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task SaveAsync(UserIdentity user, StateResult result, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
