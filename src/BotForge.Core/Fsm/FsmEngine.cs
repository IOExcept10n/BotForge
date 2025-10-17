using BotForge.Fsm.Handling;
using BotForge.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace BotForge.Fsm;

/// <summary>
/// Finite-state-machine engine that dispatches incoming updates to the appropriate handler.
/// </summary>
/// <remarks>
/// The engine inspects the provided <see cref="IUpdate"/> to determine which handler to invoke:
/// <list type="bullet">
/// <item>
/// If the update is a message (<see cref="IUpdate.IsMessage"/>), it resolves <see cref="IMessageHandler"/> from the provided <see cref="IServiceProvider"/>
/// and calls <see cref="IMessageHandler.HandleMessageAsync"/>.
/// </item>
/// <item>
/// If the update is an interaction (<see cref="IUpdate.IsInteraction"/>), it resolves <see cref="IInteractionHandler"/>
/// and calls <see cref="IInteractionHandler.HandleInteractionAsync"/>.
/// </item>
/// <item>
/// Otherwise, it attempts to resolve <see cref="IRawUpdateHandler"/> and calls <see cref="IRawUpdateHandler.HandleAsync"/> if available.
/// If no appropriate handler is registered, the method completes without performing any action.
/// </item>
/// </list>
/// </remarks>
public static class FsmEngine
{
    /// <summary>
    /// Dispatches the given <paramref name="update"/> to the appropriate handler resolved from <paramref name="services"/>.
    /// </summary>
    /// <param name="update">The update to handle. Cannot be null.</param>
    /// <param name="services">The <see cref="IServiceProvider"/> used to resolve handler services. Cannot be null.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous handling operation.</returns>
    public static async Task HandleAsync(IUpdate update, IServiceProvider services, CancellationToken cancellationToken) => await (update switch
    {
        { IsMessage: true } when services.GetService<IMessageHandler>() is IMessageHandler messageHandler => messageHandler.HandleMessageAsync(update.Message, cancellationToken),
        { IsInteraction: true } when services.GetService<IInteractionHandler>() is IInteractionHandler interactionHandler => interactionHandler.HandleInteractionAsync(update.Interaction, cancellationToken),
        _ => services.GetService<IRawUpdateHandler>()?.HandleAsync(update, cancellationToken) ?? Task.CompletedTask,
    }).ConfigureAwait(false);
}
