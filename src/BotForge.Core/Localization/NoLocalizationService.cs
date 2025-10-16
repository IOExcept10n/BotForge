using System.Globalization;

namespace BotForge.Core.Localization;
public class NoLocalizationService : ILocalizationService
{
    public string GetString(CultureInfo culture, string key) => key;
    public string GetString(CultureInfo culture, string key, params object[] args) => string.Format(culture, key, args);
}
