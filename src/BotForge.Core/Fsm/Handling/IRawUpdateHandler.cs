using BotForge.Core.Messaging;

namespace BotForge.Core.Fsm.Handling;

public interface IRawUpdateHandler
{
    Task HandleAsync(IUpdate update, CancellationToken cancellationToken);
}
