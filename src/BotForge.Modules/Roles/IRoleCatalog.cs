namespace BotForge.Modules.Roles;

public interface IRoleCatalog
{
    IEnumerable<Role> DefinedRoles { get; }

    Role DefaultRole { get; }

    IReadOnlyCollection<ModuleDescriptor> ListAvailableModules(Role role);

    string GetWelcomeMessage(Role role);
}
