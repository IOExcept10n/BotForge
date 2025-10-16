using BotForge.Core.Messaging;

namespace BotForge.Core.Fsm.Handling;

public interface IMessageHandler
{
    Task HandleMessageAsync(IMessage message, CancellationToken cancellationToken);
}
