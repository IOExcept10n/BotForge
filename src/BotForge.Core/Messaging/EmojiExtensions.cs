using System.Collections.Frozen;
using System.Reflection;

namespace BotForge.Messaging;

/// <summary>
/// Provides extension methods for built-in framework emojis.
/// </summary>
public static class EmojiExtensions
{
    /// <summary>
    /// A character for the unknown emoji code.
    /// </summary>
    public const string UnknownEmojiSign = "\ufffd";

    private static readonly EmojiMaps maps = Initialize();

    private static EmojiMaps Initialize()
    {
        List<KeyValuePair<Emoji, string>> pairsTo = [new(Emoji.None, string.Empty)];
        List<KeyValuePair<string, Emoji>> pairsFrom = [new(string.Empty, Emoji.None)];

        foreach (var field in typeof(EmojiStrings).GetFields(BindingFlags.Static | BindingFlags.Public))
        {
            string name = field.Name;
#pragma warning disable CA1308
            string enumName = string.Join("", from word in name.Split('_') select (word[0] + word[1..].ToLowerInvariant()));
            string value = (string)field.GetValue(null)!;
            Emoji code = Enum.Parse<Emoji>(enumName);
            pairsTo.Add(new(code, value));
            pairsFrom.Add(new(value, code));
        }

        return (pairsTo.ToFrozenDictionary(), pairsFrom.ToFrozenDictionary());
    }

    /// <summary>
    /// Represents <see cref="bool"/> value as corresponding <see cref="Emoji"/> value.
    /// </summary>
    /// <param name="value">A value to convert to emoji.</param>
    /// <returns>
    /// <see cref="Emoji.WhiteHeavyCheckMark"/> if the value is <see langword="true"/>; otherwise <see cref="Emoji.CrossMark"/>.
    /// </returns>
    public static Emoji AsEmoji(this bool value) => value ? Emoji.WhiteHeavyCheckMark : Emoji.CrossMark;

    /// <summary>
    /// Converts <see cref="Emoji"/> value to corresponding Unicode emoji representation.
    /// </summary>
    /// <param name="emoji">An emoji code to get Unicode emoji for.</param>
    /// <returns>Corresponding Unicode emoji for the specified code or <see cref="UnknownEmojiSign"/> if value is unknown.</returns>
    public static string ToUnicode(this Emoji emoji)
    {
        if (maps.EmojiUnicodeMap.TryGetValue(emoji, out string? value))
            return value;
        return UnknownEmojiSign;
    }

    /// <summary>
    /// Converts the text value to corresponding <see cref="Emoji"/> code representation.
    /// </summary>
    /// <param name="text">A text string to get emoji representation for.</param>
    /// <returns>Corresponding emoji code for string or <see cref="Emoji.None"/> if value is unknown.</returns>
    public static Emoji ToEmoji(this string text)
    {
        if (maps.UnicodeEmojiMap.TryGetValue(text, out Emoji value))
            return value;
        return Emoji.None;
    }
}

internal readonly record struct EmojiMaps(FrozenDictionary<Emoji, string> EmojiUnicodeMap, FrozenDictionary<string, Emoji> UnicodeEmojiMap)
{
    public static implicit operator (FrozenDictionary<Emoji, string> EmojiUnicodeMap, FrozenDictionary<string, Emoji> UnicodeEmojiMap)(EmojiMaps value)
    {
        return (value.EmojiUnicodeMap, value.UnicodeEmojiMap);
    }

    public static implicit operator EmojiMaps((FrozenDictionary<Emoji, string> EmojiUnicodeMap, FrozenDictionary<string, Emoji> UnicodeEmojiMap) value)
    {
        return new EmojiMaps(value.EmojiUnicodeMap, value.UnicodeEmojiMap);
    }
}
