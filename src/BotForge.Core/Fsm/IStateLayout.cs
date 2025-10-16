using BotForge.Core.Messaging;

namespace BotForge.Core.Fsm;

public interface IStateLayout
{
    Task SendLayoutMessageAsync(IReplyChannel channel, ChatId chatId, ReplyContext? overrideMessage = null, CancellationToken cancellationToken = default);
}
