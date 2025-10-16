using BotForge.Messaging;

namespace BotForge.Fsm;

public interface IStateLayout
{
    Task SendLayoutMessageAsync(IReplyChannel channel, ChatId chatId, ReplyContext? overrideMessage = null, CancellationToken cancellationToken = default);
}
