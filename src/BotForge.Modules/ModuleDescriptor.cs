using BotForge.Fsm;
using BotForge.Messaging;
using BotForge.Modules.Roles;

namespace BotForge.Modules;

public record ModuleDescriptor(string ModuleName, Type ModuleType, ButtonLabel ModuleButton, RoleSet AllowedRoles, IStateHandler RootState, IReadOnlyDictionary<string, IStateHandler> States);
