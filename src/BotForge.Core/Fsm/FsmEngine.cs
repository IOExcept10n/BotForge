using BotForge.Fsm.Handling;
using BotForge.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace BotForge.Fsm;

public sealed class FsmEngine
{
    public async Task HandleAsync(IUpdate update, IServiceProvider services, CancellationToken cancellationToken) => await (update switch
    {
        { IsMessage: true } when services.GetService<IMessageHandler>() is IMessageHandler messageHandler => messageHandler.HandleMessageAsync(update.Message, cancellationToken),
        { IsInteraction: true } when services.GetService<IInteractionHandler>() is IInteractionHandler interactionHandler => interactionHandler.HandleInteractionAsync(update.Interaction, cancellationToken),
        _ => services.GetService<IRawUpdateHandler>()?.HandleAsync(update, cancellationToken) ?? Task.CompletedTask,
    }).ConfigureAwait(false);
}
