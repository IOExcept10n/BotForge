using BotForge.Localization;
using BotForge.Messaging;

namespace BotForge.Modules.Layouts;

internal class MenuStateLayout : StateLayout
{
    public ReplyKeyboard? Buttons { get; init; }

    public bool DisableKeyboard { get; init; }

    public bool InheritKeyboard { get; init; }

    public override async Task SendLayoutMessageAsync(IReplyChannel channel, UserIdentity user, ILocalizationService localization, ReplyContext? overrideMessage = null, CancellationToken cancellationToken = default)
    {
        ReplyKeyboard? keyboard;

        if (InheritKeyboard)
            keyboard = null;
        else if (DisableKeyboard)
            keyboard = new([]);
        else
            keyboard = Buttons;

        await channel.SendAsync(user, BuildReply(user, localization, keyboard, overrideMessage), cancellationToken).ConfigureAwait(false);
    }
}
