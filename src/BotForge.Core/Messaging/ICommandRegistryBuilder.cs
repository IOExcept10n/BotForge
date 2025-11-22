using BotForge.Fsm.Handling;

namespace BotForge.Messaging;

public interface ICommandRegistryBuilder
{
    ICommandRegistryBuilder AddCommand(ICommandHandler handler);

    IRegistry<ICommandHandler> Build();
}
