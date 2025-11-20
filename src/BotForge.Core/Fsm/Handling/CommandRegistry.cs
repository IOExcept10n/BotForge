using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace BotForge.Fsm.Handling;

internal class CommandRegistry : IRegistry<ICommandHandler>
{
    private readonly Dictionary<string, ICommandHandler> _registry = [];

    public IEnumerator<ICommandHandler> GetEnumerator() => _registry.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Register(ICommandHandler instance)
    {
        ArgumentNullException.ThrowIfNull(instance);
        _registry[instance.CommandName] = instance;
    }

    public bool TryGet(string key, [NotNullWhen(true)] out ICommandHandler? instance) => _registry.TryGetValue(key, out instance);
}
