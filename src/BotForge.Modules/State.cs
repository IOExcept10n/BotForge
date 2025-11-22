using BotForge.Fsm;

namespace BotForge.Modules;

internal record State(StateDefinition Definition, IStateHandler Handler);
