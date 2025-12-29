using BotForge.Fsm;
using BotForge.Fsm.Handling;

namespace BotForge.Messaging;

/// <summary>
/// Provides extension methods for building a command registry in the BotForge framework.
/// </summary>
public static class CommandBuilderExtensions
{
    /// <param name="builder">The command registry builder to which the command handler is being added.</param>
    extension(ICommandRegistryBuilder builder)
    {
        /// <summary>
        /// Adds a command handler of type <typeparamref name="TCommand"/> to the command registry builder.
        /// </summary>
        /// <typeparam name="TCommand">The type of command handler to add, which must implement <see cref="ICommandHandler"/>.</typeparam>
        /// <returns>The updated command registry builder with the new command handler added.</returns>
        public ICommandRegistryBuilder AddCommand<TCommand>() where TCommand : ICommandHandler, new() => builder.AddCommand(new TCommand());

        /// <summary>
        /// Adds a command handler to the command registry builder using a specified command name and a handler function.
        /// </summary>
        /// <param name="name">A string representing the name of the command to add.</param>
        /// <param name="handler">A function to handle the command, which takes <see cref="InteractionStateContext"/> and <see cref="CancellationToken"/> 
        /// and returns a <see cref="Task{StateResult}"/>.</param>
        /// <returns>The updated command registry builder with the new command handler added.</returns>
        public ICommandRegistryBuilder AddCommand(string name, Func<InteractionStateContext, CancellationToken, Task<StateResult>> handler) => builder.AddCommand(new CommandHandler(handler) { CommandName = name });
    }

    private class CommandHandler(Func<InteractionStateContext, CancellationToken, Task<StateResult>> handler) : ICommandHandler
    {
        private readonly Func<InteractionStateContext, CancellationToken, Task<StateResult>> _handler = handler;

        public required string CommandName { get; set; }

        public async Task<StateResult> HandleCommand(InteractionStateContext ctx, CancellationToken cancellationToken = default) => await _handler(ctx, cancellationToken).ConfigureAwait(false);
    }
}
