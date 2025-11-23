using System.Globalization;
using BotForge.Messaging;

namespace BotForge.Localization;

public interface IUserLocaleProvider
{
    Task<CultureInfo?> GetPreferredLocaleAsync(UserIdentity user, CancellationToken cancellationToken);

    Task SetPreferredLocaleAsync(UserIdentity user, CultureInfo? preferredLocale, CancellationToken cancellationToken);
}
