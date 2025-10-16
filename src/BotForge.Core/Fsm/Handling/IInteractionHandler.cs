using BotForge.Messaging;

namespace BotForge.Fsm.Handling;

public interface IInteractionHandler
{
    Task HandleInteractionAsync(IInteraction interaction, CancellationToken cancellationToken);
}
