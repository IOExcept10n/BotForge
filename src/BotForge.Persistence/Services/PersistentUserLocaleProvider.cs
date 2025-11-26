using System.Globalization;
using BotForge.Messaging;
using BotForge.Persistence.Repositories;
using BotForge.Localization;

namespace BotForge.Persistence.Services;

public class PersistentUserLocaleProvider(IBotUserRepository _users) : IUserLocaleProvider
{
    public async Task<CultureInfo?> GetPreferredLocaleAsync(UserIdentity user, CancellationToken cancellationToken)
    {
        var botUser = await _users.GetByPlatformIdAsync(user.Id, cancellationToken).ConfigureAwait(false);
        if (botUser is null || string.IsNullOrEmpty(botUser.PreferredLocale))
            return null;
        try
        {
            return new CultureInfo(botUser.PreferredLocale!);
        }
        catch
        {
            return null;
        }
    }

    public async Task SetPreferredLocaleAsync(UserIdentity user, CultureInfo? preferredLocale, CancellationToken cancellationToken)
    {
        var botUser = await _users.GetOrCreateByPlatformIdAsync(user.Id, cancellationToken).ConfigureAwait(false);
        botUser.PreferredLocale = preferredLocale?.Name;
        await _users.UpdateAsync(botUser, cancellationToken).ConfigureAwait(false);
        await _users.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
