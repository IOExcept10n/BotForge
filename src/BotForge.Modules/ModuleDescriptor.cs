using BotForge.Fsm;
using BotForge.Modules.Roles;

namespace BotForge.Modules;

public record ModuleDescriptor(string ModuleName, Type ModuleType, RoleSet AllowedRoles, IStateHandler RootState, IReadOnlyDictionary<string, IStateHandler> States);
