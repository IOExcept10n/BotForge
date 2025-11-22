using BotForge.Fsm;
using BotForge.Fsm.Handling;

namespace BotForge.Messaging;

public static class CommandBuilderExtensions
{
    extension(ICommandRegistryBuilder builder)
    {
        public ICommandRegistryBuilder AddCommand<TCommand>() where TCommand : ICommandHandler, new() => builder.AddCommand(new TCommand());

        public ICommandRegistryBuilder AddCommand(string name, Func<InteractionStateContext, CancellationToken, Task<StateResult>> handler) => builder.AddCommand(new CommandHandler(handler));
    }

    private class CommandHandler(Func<InteractionStateContext, CancellationToken, Task<StateResult>> handler) : ICommandHandler
    {
        private readonly Func<InteractionStateContext, CancellationToken, Task<StateResult>> _handler = handler;

        public string CommandName { get; set; }

        public async Task<StateResult> HandleCommand(InteractionStateContext ctx, CancellationToken cancellationToken = default) => await _handler(ctx, cancellationToken).ConfigureAwait(false);
    }
}
