using System.Globalization;
using BotForge.Fsm;
using BotForge.Localization;
using BotForge.Messaging;

namespace BotForge.Modules.Layouts;

internal abstract class StateLayout : IStateLayout
{
    public string MessageKey { get; init; } = string.Empty;

    public abstract Task SendLayoutMessageAsync(IReplyChannel channel, UserIdentity user, ILocalizationService localization, ReplyContext? overrideMessage = null, CancellationToken cancellationToken = default);

    protected ReplyContext BuildReply(UserIdentity user, ILocalizationService localization, ReplyKeyboard? keyboard, ReplyContext? overrideMessage) =>
        new(overrideMessage?.Message ?? localization.GetString(user.Locale ?? CultureInfo.InvariantCulture, MessageKey),
                overrideMessage?.Keyboard ?? keyboard);
}
