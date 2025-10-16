using System.Globalization;
using System.Resources;

namespace BotForge.Core.Localization;

/// <summary>
/// Implements the localization service using a resource manager to provide localized strings.
/// </summary>
/// <remarks>
/// This implementation stores user language preferences in memory and uses a resource manager
/// to fetch localized strings from embedded resources.
/// </remarks>
/// <param name="resourceManager">The resource manager used to fetch localized strings.</param>
public class ResourceLocalizationService(ResourceManager resourceManager) : ILocalizationService
{
    private readonly ResourceManager _resourceManager = resourceManager;

    /// <inheritdoc />
    public string GetString(CultureInfo culture, string key) => _resourceManager.GetString(key, culture) ?? key;

    /// <inheritdoc />
    public string GetString(CultureInfo culture, string key, params object[] args)
    {
        var format = GetString(culture, key);
        return string.Format(culture, format, args);
    }
}
