using System.Globalization;

namespace BotForge.Localization;

/// <summary>
/// Provides methods for retrieving localized strings and managing a user's language preference.
/// </summary>
public interface ILocalizationService
{
    /// <summary>
    /// Asynchronously retrieves a localized string for the specified key and user.
    /// </summary>
    /// <param name="culture">Information about culture to localize the message.</param>
    /// <param name="key">The resource key identifying the localized string. Cannot be null or empty.</param>
    /// <returns>
    /// The result contains the localized string. If the key is not found a fallback or the key itself may be returned depending on implementation.
    /// </returns>
    string GetString(CultureInfo culture, string key);

    /// <summary>
    /// Asynchronously retrieves a formatted localized string for the specified key and user, using the provided arguments.
    /// </summary>
    /// <param name="culture">Information about culture to localize the message.</param>
    /// <param name="key">The resource key identifying the localized string. Cannot be null or empty.</param>
    /// <param name="args">Optional format arguments to be applied to the localized string (as in <c>string.Format</c>).</param>
    /// <returns>
    /// The result contains the formatted localized string. If the key is not found a fallback or the key itself may be returned depending on implementation.
    /// </returns>
    string GetString(CultureInfo culture, string key, params object[] args);
}
