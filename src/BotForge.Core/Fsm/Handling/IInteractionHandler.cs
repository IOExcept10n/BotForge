using BotForge.Core.Messaging;

namespace BotForge.Core.Fsm.Handling;

public interface IInteractionHandler
{
    Task HandleInteractionAsync(IInteraction interaction, CancellationToken cancellationToken);
}
