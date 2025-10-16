using BotForge.Core.Messaging;

namespace BotForge.Core.Fsm.Handling;

internal sealed class MessageHandler(
    IUserStateStore stateStore,
    IStateHandlerResolver handlerResolver,
    IStateReplyDispatcher replies,
    IServiceProvider services) : IMessageHandler
{
    private readonly IUserStateStore _stateStore = stateStore;
    private readonly IStateHandlerResolver _handlerResolver = handlerResolver;
    private readonly IStateReplyDispatcher _replies = replies;
    private readonly IServiceProvider _services = services;

    public async Task HandleMessageAsync(IMessage message, CancellationToken cancellationToken)
    {
        UserIdentity user = message.From;
        var state = await _stateStore.GetUserStateAsync(user, cancellationToken).ConfigureAwait(false);
        var context = new MessageStateContext(message, state, _services);
        if (!_handlerResolver.TryResolve(state.Id, out var handler))
        {
            handler = await TryGetUserRootStateHandlerAsync(user, cancellationToken).ConfigureAwait(false);
            if (handler == null)
                return;
        }

        StateResult result = await handler.ExecuteAsync(context, cancellationToken).ConfigureAwait(false);

        await _stateStore.SaveAsync(user, result, cancellationToken).ConfigureAwait(false);
        await _replies.SendAsync(result, context, cancellationToken).ConfigureAwait(false);
    }

    private async Task<IStateHandler?> TryGetUserRootStateHandlerAsync(UserIdentity user, CancellationToken cancellationToken)
    {
        var state = await _stateStore.GetUserRootStateAsync(user, cancellationToken).ConfigureAwait(false);
        if (!_handlerResolver.TryResolve(state.Id, out var handler))
        {
            return null;
        }
        return handler;
    }
}
