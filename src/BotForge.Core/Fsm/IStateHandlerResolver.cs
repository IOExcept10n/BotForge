using System.Diagnostics.CodeAnalysis;

namespace BotForge.Core.Fsm;

public interface IStateHandlerResolver
{
    bool TryResolve(string stateId, [NotNullWhen(true)] out IStateHandler? handler);
}
