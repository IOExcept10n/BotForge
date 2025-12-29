using System.Diagnostics.CodeAnalysis;
using BotForge.Fsm;

namespace BotForge.Modules;

internal sealed class ModuleStateHandlerResolver(IRegistry<State> states) : IStateHandlerResolver
{
    private readonly IRegistry<State> _states = states;

    public bool TryResolve(string stateId, [NotNullWhen(true)] out IStateHandler? handler)
    {
        if (_states.TryGet(stateId, out var state))
        {
            handler = state.Handler;
            return true;
        }
        handler = null;
        return false;
    }
}
