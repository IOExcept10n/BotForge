using System.Globalization;
using BotForge.Localization;
using BotForge.Messaging;
using BotForge.Persistence.Repositories;

namespace BotForge.Persistence.Services;

internal class PersistentUserLocaleProvider(IBotUserRepository users) : IUserLocaleProvider
{
    private readonly IBotUserRepository _users = users;

    public async Task<CultureInfo?> GetPreferredLocaleAsync(UserIdentity user, CancellationToken cancellationToken)
    {
        var botUser = await _users.GetOrRegisterAsync(user, cancellationToken).ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(botUser.PreferredLocale))
            return null;
        return new CultureInfo(botUser.PreferredLocale!);
    }

    public async Task SetPreferredLocaleAsync(UserIdentity user, CultureInfo? preferredLocale, CancellationToken cancellationToken)
    {
        var botUser = await _users.GetOrRegisterAsync(user, cancellationToken).ConfigureAwait(false);
        botUser.PreferredLocale = preferredLocale?.Name;
        await _users.UpdateAsync(botUser, cancellationToken).ConfigureAwait(false);
        await _users.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
