using System.Diagnostics.CodeAnalysis;

namespace BotForge.Core.Fsm;

internal class StateRegistry : IRegistry<StateDefinition>
{
    private readonly Dictionary<string, StateDefinition> _registry = [];

    public void Register(StateDefinition def)
    {
        ArgumentNullException.ThrowIfNull(def);
        _registry.Add(def.StateId, def);
    }

    public bool TryGet(string stateId, [NotNullWhen(true)] out StateDefinition? def)
        => _registry.TryGetValue(stateId, out def);
}
