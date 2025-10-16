using BotForge.Messaging;

namespace BotForge.Fsm.Handling;

public interface IMessageHandler
{
    Task HandleMessageAsync(IMessage message, CancellationToken cancellationToken);
}
