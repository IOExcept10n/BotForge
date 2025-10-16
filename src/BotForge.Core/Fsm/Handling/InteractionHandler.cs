using BotForge.Core.Messaging;

namespace BotForge.Core.Fsm.Handling;

internal sealed class InteractionHandler(
    IUserStateStore stateStore,
    IRegistry<ICommandHandler> commandRegistry,
    IStateReplyDispatcher replies,
    IServiceProvider services) : IInteractionHandler
{
    private readonly IUserStateStore _stateStore = stateStore;
    private readonly IRegistry<ICommandHandler> _commandRegistry = commandRegistry;
    private readonly IStateReplyDispatcher _replies = replies;
    private readonly IServiceProvider _services = services;

    public async Task HandleInteractionAsync(IInteraction interaction, CancellationToken cancellationToken)
    {
        UserIdentity user = interaction.From;
        var state = await _stateStore.GetUserStateAsync(user, cancellationToken).ConfigureAwait(false);
        var context = new InteractionStateContext(interaction, state, _services);

        StateResult? result = null;
        switch (interaction.Type)
        {
            case InteractionType.Command when !string.IsNullOrEmpty(interaction.CommandName):
                if (_commandRegistry.TryGet(interaction.CommandName, out var command))
                {
                    result = await command.HandleCommand(context, cancellationToken).ConfigureAwait(false);
                }
                break;
        }

        if (result == null)
        {
            return;
        }

        await _stateStore.SaveAsync(user, result, cancellationToken).ConfigureAwait(false);
        await _replies.SendAsync(result, context, cancellationToken).ConfigureAwait(false);
    }
}
