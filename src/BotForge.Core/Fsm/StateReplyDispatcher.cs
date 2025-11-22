using BotForge.Localization;
using BotForge.Messaging;

namespace BotForge.Fsm;

internal class StateReplyDispatcher(IReplyChannel channel, IRegistry<StateDefinition> states, ILocalizationService localization) : IStateReplyDispatcher
{
    private readonly IReplyChannel _channel = channel;
    private readonly IRegistry<StateDefinition> _states = states;
    private readonly ILocalizationService _localization = localization;

    public async Task SendAsync(UserIdentity user, StateResult result, StateContext ctx, CancellationToken cancellationToken = default)
    {
        if (!_states.TryGet(result.NextStateId, out var stateDefinition))
            throw new InvalidOperationException("Couldn't get reply message for not existing state.");

        await stateDefinition.Layout.SendLayoutMessageAsync(_channel, user, _localization, result.OverrideNextStateMessage, cancellationToken).ConfigureAwait(false);
    }
}
