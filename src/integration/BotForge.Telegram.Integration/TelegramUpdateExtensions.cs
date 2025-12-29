using BotForge.Localization;
using BotForge.Messaging;
using Telegram.Bot.Types;

namespace BotForge.Telegram.Integration;

/// <summary>
/// Provides extension methods for Telegram native updates.
/// </summary>
public static class TelegramUpdateExtensions
{
    /// <summary>
    /// Converts a Telegram <see cref="Update"/> to a BotForge <see cref="IUpdate"/> using <see cref="TelegramUpdateParser"/>.
    /// </summary>
    public static async Task<IUpdate> ToBotForgeAsync(this Update update, IUserLocaleProvider? localeProvider, CancellationToken cancellationToken)
        => await TelegramUpdateParser.ParseAsync(update, localeProvider, cancellationToken).ConfigureAwait(false);
}
