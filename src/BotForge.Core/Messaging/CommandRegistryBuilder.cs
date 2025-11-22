using BotForge.Fsm.Handling;

namespace BotForge.Messaging;

internal class CommandRegistryBuilder : ICommandRegistryBuilder
{
    private readonly CommandRegistry _registry = new();

    public ICommandRegistryBuilder AddCommand(ICommandHandler handler)
    {
        _registry.Register(handler);
        return this;
    }

    public IRegistry<ICommandHandler> Build() => _registry;
}
