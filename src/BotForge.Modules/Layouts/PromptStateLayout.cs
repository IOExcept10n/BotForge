using BotForge.Localization;
using BotForge.Messaging;

namespace BotForge.Modules.Layouts;

internal sealed class PromptStateLayout(ButtonLabel cancelButton) : StateLayout
{
    public bool AllowCancel { get; set; }

    public override async Task SendLayoutMessageAsync(IReplyChannel channel, UserIdentity user, ILocalizationService localization, ReplyContext? overrideMessage = null, CancellationToken cancellationToken = default) =>
        await channel.SendAsync(user, BuildReply(user, localization, AllowCancel ? new ReplyKeyboard([[cancelButton]]) : null, overrideMessage), cancellationToken).ConfigureAwait(false);
}
