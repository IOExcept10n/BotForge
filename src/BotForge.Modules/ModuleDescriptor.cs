using BotForge.Fsm;
using BotForge.Messaging;
using BotForge.Modules.Roles;

namespace BotForge.Modules;

/// <summary>
/// Represents a descriptor for a module in the BotForge framework.
/// </summary>
/// <param name="ModuleName">The name of the module.</param>
/// <param name="ModuleType">The <see cref="Type"/> of the module, representing its implementation.</param>
/// <param name="ModuleButton">The <see cref="ButtonLabel"/> associated with the module, used for UI representation.</param>
/// <param name="AllowedRoles">The set of roles that are permitted to access the module.</param>
/// <param name="RootState">The <see cref="IStateHandler"/> that serves as the root state for the module's state machine part.</param>
/// <param name="Order">An integer representing the order of the module in a menu.</param>
/// <param name="Display">A boolean indicating whether the module should be displayed in a UI context.</param>
/// <param name="States">A dictionary of states keyed by their identifiers, with <see cref="IStateHandler"/> instances representing each state.</param>
public record ModuleDescriptor(
    string ModuleName,
    Type ModuleType,
    ButtonLabel ModuleButton,
    RoleSet AllowedRoles,
    IStateHandler RootState,
    int Order,
    bool Display,
    IReadOnlyDictionary<string, IStateHandler> States);
