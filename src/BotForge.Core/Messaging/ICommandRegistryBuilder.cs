using BotForge.Fsm.Handling;

namespace BotForge.Messaging;

/// <summary>
/// Defines a builder for registering command handlers in the BotForge framework.
/// </summary>
public interface ICommandRegistryBuilder
{
    /// <summary>
    /// Adds a command handler to the registry.
    /// </summary>
    /// <param name="handler">An instance of <see cref="ICommandHandler"/> to be added to the registry.</param>
    /// <returns>
    /// The updated <see cref="ICommandRegistryBuilder"/> instance with the new handler added.
    /// </returns>
    ICommandRegistryBuilder AddCommand(ICommandHandler handler);

    /// <summary>
    /// Builds and returns the command registry, finalizing the registration of command handlers.
    /// </summary>
    /// <returns>
    /// An instance of <see cref="IRegistry{ICommandHandler}"/> containing all registered command handlers.
    /// </returns>
    IRegistry<ICommandHandler> Build();
}
