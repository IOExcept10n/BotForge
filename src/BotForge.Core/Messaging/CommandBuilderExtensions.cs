using System;
using System.Collections.Generic;
using System.Text;
using BotForge.Fsm;
using BotForge.Fsm.Handling;

namespace BotForge.Messaging;

public static class CommandBuilderExtensions
{
    extension(ICommandRegistryBuilder builder)
    {
        ICommandRegistryBuilder AddCommand<TCommand>() where TCommand : ICommandHandler, new() => builder.AddCommand(new TCommand());

        ICommandRegistryBuilder AddCommand(string name, Func<InteractionStateContext, CancellationToken, Task<StateResult>> handler) => builder.AddCommand();
    }

    private class CommandHandler : ICommandHandler
    {

    }
}
