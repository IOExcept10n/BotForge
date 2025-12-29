using System.Globalization;
using BotForge.Messaging;

namespace BotForge.Localization;

/// <summary>
/// Provides methods to manage user locale preferences in a bot application.
/// </summary>
public interface IUserLocaleProvider
{
    /// <summary>
    /// Retrieves the user's preferred locale asynchronously.
    /// </summary>
    /// <param name="user">An instance of <see cref="UserIdentity"/> representing the user for whom the preferred locale is requested.</param>
    /// <param name="cancellationToken">A token that allows the operation to be cancelled.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a <see cref="CultureInfo"/> object representing the user's preferred locale,
    /// or <c>null</c> if no preferred locale is set.
    /// </returns>
    Task<CultureInfo?> GetPreferredLocaleAsync(UserIdentity user, CancellationToken cancellationToken);

    /// <summary>
    /// Changes user locale to the specific <paramref name="preferredLocale"/>.
    /// </summary>
    /// <remarks>
    /// Note that the locale changes are taken into action only when the user sends the next message to the bot.
    /// This way, the current context and reply messages/markup will remain with the old locale until the user
    /// writes a new message.
    /// </remarks>
    /// <param name="user">An instance of <see cref="UserIdentity"/> to use locale with.</param>
    /// <param name="preferredLocale">A new <see cref="CultureInfo"/> object representing the preferred locale to use
    /// other than the system defaults.</param>
    /// <param name="cancellationToken">A token that allows the operation to be cancelled.</param>
    /// <returns>An asynchronous task representing the locale saving action.</returns>
    Task SetPreferredLocaleAsync(UserIdentity user, CultureInfo? preferredLocale, CancellationToken cancellationToken);
}
