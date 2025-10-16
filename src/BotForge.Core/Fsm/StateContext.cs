using BotForge.Core.Messaging;

namespace BotForge.Core.Fsm;

public record StateContext(StateRecord CurrentState, IServiceProvider Services);

public record MessageStateContext(IMessage Message, StateRecord currentState, IServiceProvider services)
    : StateContext(currentState, services);

public record InteractionStateContext(IInteraction Interaction, StateRecord currentState, IServiceProvider services)
    : StateContext(currentState, services);
