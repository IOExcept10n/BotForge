using System.Collections.Concurrent;
using System.Globalization;
using BotForge.Messaging;

namespace BotForge.Localization;

internal class InMemoryUserLocaleProvider : IUserLocaleProvider
{
    private readonly ConcurrentDictionary<long, CultureInfo> _locales = [];

    public Task<CultureInfo?> GetPreferredLocaleAsync(UserIdentity user, CancellationToken cancellationToken)
    {
        _locales.TryGetValue(user.Id, out var locale);
        return Task.FromResult(locale);
    }

    public Task SetPreferredLocaleAsync(UserIdentity user, CultureInfo? preferredLocale, CancellationToken cancellationToken)
    {
        if (preferredLocale == null)
            _locales.TryRemove(user.Id, out _);
        else
            _locales[user.Id] = preferredLocale;
        return Task.CompletedTask;
    }
}
