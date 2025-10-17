using BotForge.Messaging;

namespace BotForge.Fsm;

/// <summary>
/// Context provided to components that need access to the user's current <see cref="StateRecord"/>
/// and the application's <see cref="IServiceProvider"/> for resolving services.
/// </summary>
/// <param name="CurrentState">The user's current <see cref="StateRecord"/>.</param>
/// <param name="Services">The service provider used to resolve services required during state processing.</param>
public record StateContext(StateRecord CurrentState, IServiceProvider Services);

/// <summary>
/// Context passed to state handlers that process incoming messages.
/// Inherits from <see cref="StateContext"/> and adds the incoming <see cref="IMessage"/>.
/// </summary>
/// <param name="Message">The incoming message to process.</param>
/// <param name="currentState">The user's current <see cref="StateRecord"/>.</param>
/// <param name="services">The service provider used to resolve services required during state processing.</param>
public record MessageStateContext(IMessage Message, StateRecord currentState, IServiceProvider services)
    : StateContext(currentState, services);

/// <summary>
/// Context passed to state handlers that process interactions (for example, button presses).
/// Inherits from <see cref="StateContext"/> and adds the incoming <see cref="IInteraction"/>.
/// </summary>
/// <param name="Interaction">The incoming interaction to process.</param>
/// <param name="currentState">The user's current <see cref="StateRecord"/>.</param>
/// <param name="services">The service provider used to resolve services required during state processing.</param>
public record InteractionStateContext(IInteraction Interaction, StateRecord currentState, IServiceProvider services)
    : StateContext(currentState, services);
