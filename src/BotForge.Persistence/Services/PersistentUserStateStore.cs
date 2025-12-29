using BotForge.Fsm;
using BotForge.Messaging;
using BotForge.Modules.Roles;
using BotForge.Persistence.Models;
using BotForge.Persistence.Repositories;

namespace BotForge.Persistence.Services;

internal sealed class PersistentUserStateStore(IBotUserRepository users, IRoleCatalog roleCatalog) : IUserStateStore
{
    private readonly IBotUserRepository _users = users;
    private readonly IRoleCatalog _roleCatalog = roleCatalog;

    public async Task<StateRecord> GetUserRootStateAsync(UserIdentity user, CancellationToken ct = default)
    {
        // try to get user and its role, fall back to catalog default
        var botUser = await GetOrRegisterAsync(user, ct).ConfigureAwait(false);
        string roleName = botUser?.Role?.Name ?? _roleCatalog.DefaultRole.Name;
        return new StateRecord($"{roleName}:{StateRecord.StartStateId}", string.Empty);
    }

    public async Task<StateRecord> GetUserStateAsync(UserIdentity user, CancellationToken ct = default)
    {
        var botUser = await GetOrRegisterAsync(user, ct).ConfigureAwait(false);
        if (botUser.State != null)
        {
            return new StateRecord(botUser.State.StateId ?? StateRecord.StartStateId, botUser.State.StateData ?? string.Empty);
        }
        return StateRecord.StartState;
    }

    public async Task SaveAsync(UserIdentity user, StateResult result, CancellationToken ct = default)
    {
        var botUser = await GetOrRegisterAsync(user, ct).ConfigureAwait(false);

        // update or create user state
        var now = DateTimeOffset.UtcNow;
        if (botUser.State == null)
        {
            // Create new state and assign through navigation property
            // EF Core will automatically set the FK when saving
            // Important: Don't set UserId explicitly - let EF Core handle it through navigation property
            botUser.State = new UserState
            {
                StateId = result.NextStateId,
                StateData = result.NextStateData,
                UpdatedAt = now
            };
        }
        else
        {
            // Update existing state - EF Core will track changes automatically
            botUser.State.StateId = result.NextStateId;
            botUser.State.StateData = result.NextStateData;
            botUser.State.UpdatedAt = now;
        }

        // This is needed to prevent PostgreSQL jsonb from throwing an error in case state don't have a value to save.
        // Note that this can break user logic.
        if (string.IsNullOrWhiteSpace(botUser.State.StateData))
        {
            botUser.State.StateData = "\"\"";
        }

        botUser.LastSeen = now;

        await _users.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    public async Task<BotUser> GetOrRegisterAsync(UserIdentity user, CancellationToken ct = default)
    {
        return await _users.GetOrRegisterAsync(user, ct).ConfigureAwait(false);
    }
}
