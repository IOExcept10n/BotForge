using System.Globalization;
using BotForge.Localization;
using BotForge.Messaging;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotForge.Telegram.Integration;

internal class TelegramReplyChannel(ITelegramBotClient client, ILocalizationService localization) : IReplyChannel
{
    public async Task RemoveKeyboardAsync(UserIdentity user, CancellationToken ct = default)
    {
        var msg = await client.SendMessage(user.Id, "<remove keyboard>", replyMarkup: new ReplyKeyboardRemove(), cancellationToken: ct).ConfigureAwait(false);
        await client.DeleteMessage(user.Id, msg.MessageId, ct).ConfigureAwait(false);
    }

    public async Task SendAsync(UserIdentity user, ReplyContext ctx, CancellationToken ct = default) =>
        await client.SendMessage(user.Id, ctx.Message ?? string.Empty, replyMarkup: ToMarkup(ctx.Keyboard, user.TargetLocale), cancellationToken: ct).ConfigureAwait(false);

    private ReplyMarkup? ToMarkup(ReplyKeyboard? keyboard, CultureInfo culture)
    {
        if (keyboard == null)
            return null;
        if (!keyboard.Buttons.Any())
            return new ReplyKeyboardRemove();

        return new ReplyKeyboardMarkup(keyboard.Buttons.Select(x => x.Select(y => new KeyboardButton(y.Localize(localization, culture)))))
        {
            OneTimeKeyboard = keyboard.OneTime,
            ResizeKeyboard = keyboard.Resize,
        };
    }
}
