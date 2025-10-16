using BotForge.Messaging;

namespace BotForge.Fsm.Handling;

public interface IRawUpdateHandler
{
    Task HandleAsync(IUpdate update, CancellationToken cancellationToken);
}
