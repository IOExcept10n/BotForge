using System.Globalization;
using BotForge.Fsm;
using BotForge.Messaging;
using BotForge.Modules.Roles;
using BotForge.Persistence.Models;
using BotForge.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BotForge.Persistence.Services;

internal class PersistentUserStateStore(IBotUserRepository users, IRoleCatalog roleCatalog) : IUserStateStore
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
        int attempts = 0;
        while (true)
        {
            var botUser = await GetOrRegisterAsync(user, ct).ConfigureAwait(false);

            // update or create user state
            var now = DateTimeOffset.UtcNow;
            if (botUser.State == null)
            {
                var state = new UserState
                {
                    Id = Guid.NewGuid(),
                    UserId = botUser.Id,
                    StateId = result.NextStateId,
                    StateData = result.NextStateData,
                    UpdatedAt = now
                };
                botUser.State = state;
            }
            else
            {
                botUser.State.StateId = result.NextStateId;
                botUser.State.StateData = result.NextStateData;
                botUser.State.UpdatedAt = now;
            }

            botUser.LastSeen = now;

            try
            {
                await _users.UpdateAsync(botUser, ct).ConfigureAwait(false);
                await _users.SaveChangesAsync(ct).ConfigureAwait(false);
                return;
            }
            catch (DbUpdateConcurrencyException) when (attempts == 0)
            {
                // Retry once with freshly loaded data
                attempts++;
                continue;
            }
        }
    }

    public async Task<BotUser> GetOrRegisterAsync(UserIdentity user, CancellationToken ct = default)
    {
        return await _users.GetOrRegisterAsync(user, ct).ConfigureAwait(false);
    }
}
