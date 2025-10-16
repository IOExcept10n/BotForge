using BotForge.Core.Messaging;
using Xunit;

namespace BotForge.Core.Tests;

public class ButtonAndEmojiTests
{
    [Fact]
    public void ButtonLabel_Parse_WithEmoji()
    {
        var heart = Emoji.HeartDecoration; // some known emoji
        string text = EmojiStrings.HEART_DECORATION + " Title";
        var label = ButtonLabel.Parse(text, null);

        Assert.Equal("Title", label.TitleKey);
        Assert.NotEqual(Emoji.None, label.Emoji);
    }

    [Fact]
    public void EmojiExtensions_AsEmoji_ToUnicode_Roundtrip()
    {
        var e = true.AsEmoji();
        var uni = e.ToUnicode();
        var parsed = uni.ToEmoji();
        Assert.Equal(e, parsed);
    }
}
