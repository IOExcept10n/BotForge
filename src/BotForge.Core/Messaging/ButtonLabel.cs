using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using BotForge.Core.Localization;

namespace BotForge.Core.Messaging;

/// <summary>
/// Represents information about label on interactive menu button for chatbot.
/// </summary>
/// <param name="Emoji">Emoji to display on button.</param>
/// <param name="TitleKey">Key to translated title to display on button.</param>
public sealed record ButtonLabel(Emoji Emoji, string TitleKey)
    : IParsable<ButtonLabel>, IEqualityOperators<ButtonLabel, ButtonLabel, bool>
{
    /// <summary>
    /// Gets localized button text for the specified target culture.
    /// </summary>
    /// <param name="localizationService">An instance of the localization service to localize button text.</param>
    /// <param name="targetCulture">Culture info to localize text to.</param>
    /// <returns>Text prepared for the specified culture.</returns>
    public string Localize(ILocalizationService localizationService, CultureInfo targetCulture)
    {
        ArgumentNullException.ThrowIfNull(localizationService);
        if (Emoji == Emoji.None)
            return localizationService.GetString(targetCulture, TitleKey);
        return $"{Emoji.ToUnicode()} {localizationService.GetString(targetCulture, TitleKey)}";
    }

    /// <summary>
    /// Gets the button label from its string representation.
    /// </summary>
    /// <param name="text">Text to convert to button label.</param>
    /// <returns>Button label instance with data from the specified string.</returns>
    public static implicit operator ButtonLabel(string text) => Parse(text, null);

    /// <summary>
    /// Gets the button label from its string representation.
    /// </summary>
    /// <param name="text">Text to convert to button label.</param>
    /// <returns>Button label instance with data from the specified string.</returns>
    public static ButtonLabel FromString(string text) => Parse(text, null);

    /// <inheritdoc/>
    public static ButtonLabel Parse(string s, IFormatProvider? provider)
    {
        ArgumentNullException.ThrowIfNull(s);

        try
        {
            var enumerator = StringInfo.GetTextElementEnumerator(s);
            Emoji emoji = Emoji.None;
            string title = s;

            if (enumerator.MoveNext())
            {
                var firstElement = enumerator.GetTextElement();
                emoji = firstElement.ToEmoji();
                if (emoji != Emoji.None)
                {
                    title = s[firstElement.Length..].TrimStart();
                }
            }

            return new(emoji, title);
        }
        catch (Exception ex)
        {
            throw new FormatException("Could not parse button label instance from the specified string.", ex);
        }
    }

    /// <inheritdoc/>
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [NotNullWhen(true)] out ButtonLabel? result)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            result = null;
            return false;
        }

        try
        {
            result = Parse(s, provider);
            return true;
        }
        catch (FormatException)
        {
            result = null;
            return false;
        }
    }
}
