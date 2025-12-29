using BotForge.Fsm;

namespace BotForge.Modules;

internal sealed record State(StateDefinition Definition, IStateHandler Handler);
