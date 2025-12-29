using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace BotForge.Modules;

internal sealed class StateRegistry : IRegistry<State>
{
    private readonly Dictionary<string, State> _registry = [];

    public IEnumerator<State> GetEnumerator() => _registry.Values.GetEnumerator();

    public void Register(State instance) => _registry[instance.Definition.StateId] = instance;

    public bool TryGet(string key, [NotNullWhen(true)] out State? instance) => _registry.TryGetValue(key, out instance);

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
