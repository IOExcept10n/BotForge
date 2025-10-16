using BotForge.Fsm.Handling;
using BotForge.Messaging;

namespace BotForge.Fsm;

public sealed class FsmEngine(IMessageHandler? messageHandler, IInteractionHandler? interactionHandler, IRawUpdateHandler? updateHandler)
{
    private readonly IMessageHandler? _messageHandler = messageHandler;
    private readonly IInteractionHandler? _interactionHandler = interactionHandler;
    private readonly IRawUpdateHandler? _updateHandler = updateHandler;

    public async Task HandleAsync(IUpdate update, CancellationToken cancellationToken) => await (update switch
    {
        { IsMessage: true } when _messageHandler is not null => _messageHandler.HandleMessageAsync(update.Message, cancellationToken),
        { IsInteraction: true } when _interactionHandler is not null => _interactionHandler.HandleInteractionAsync(update.Interaction, cancellationToken),
        _ => _updateHandler?.HandleAsync(update, cancellationToken) ?? Task.CompletedTask,
    }).ConfigureAwait(false);
}
