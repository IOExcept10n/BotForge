using System.Globalization;

namespace BotForge.Telegram.InformationalBot.Services;

/// <summary>
/// Defines a service for retrieving news content in different locales.
/// </summary>
internal interface INewsService
{
    /// <summary>
    /// Asynchronously retrieves news content as a string formatted for a specific locale.
    /// </summary>
    /// <param name="targetLocale">The <see cref="CultureInfo"/> representing the desired locale for the news content.</param>
    /// <param name="ct">A token that allows the operation to be canceled.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a string representing the news content tailored to the specified locale.
    /// </returns>
    Task<string> GetNewsStringAsync(CultureInfo targetLocale, CancellationToken ct);
}
