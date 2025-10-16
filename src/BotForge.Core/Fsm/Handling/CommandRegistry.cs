using System.Diagnostics.CodeAnalysis;

namespace BotForge.Core.Fsm.Handling;

internal class CommandRegistry : IRegistry<ICommandHandler>
{
    private readonly Dictionary<string, ICommandHandler> _registry = [];

    public void Register(ICommandHandler instance)
    {
        ArgumentNullException.ThrowIfNull(instance);
        _registry[instance.CommandName] = instance;
    }

    public bool TryGet(string key, [NotNullWhen(true)] out ICommandHandler? instance) => _registry.TryGetValue(key, out instance);
}
