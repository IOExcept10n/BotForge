namespace BotForge.Modules.Roles;

internal class RoleCatalogBuilder(IRegistry<ModuleDescriptor> moduleRegistry, IMainMenuConfigurator mainMenuConfigurator) : IRoleCatalogBuilder
{
    private readonly RoleCatalog _catalog = new(moduleRegistry);
    private readonly IMainMenuConfigurator _mainMenuConfigurator = mainMenuConfigurator;

    public IRoleCatalogBuilder AddRole(Role role, string welcomeMessageKey)
    {
        _catalog.Add(role, welcomeMessageKey);
        return this;
    }

    public IRoleCatalog Build()
    {
        foreach (var role in _catalog.DefinedRoles)
        {
            _mainMenuConfigurator.AddMainMenu(_catalog, role);
        }
        return _catalog;
    }

    public IRoleCatalogBuilder SetDefaultRole(Role role)
    {
        _catalog.DefaultRole = role;
        return this;
    }

    private sealed class RoleCatalog(IRegistry<ModuleDescriptor> moduleRegistry) : IRoleCatalog
    {
        private readonly IRegistry<ModuleDescriptor> _moduleRegistry = moduleRegistry;
        private readonly Dictionary<Role, List<ModuleDescriptor>> _availableModules = [];
        private readonly Dictionary<Role, string> _messages = [];
        private readonly List<Role> _definedRoles = [];

        public Role DefaultRole { get; set; } = Role.Unknown;

        public IEnumerable<Role> DefinedRoles => _definedRoles;

        public void Add(Role role, string welcome)
        {
            _definedRoles.Add(role);
            _availableModules[role] = [.. _moduleRegistry.Where(x => x.AllowedRoles.Contains(role))];
            _messages[role] = welcome;
        }

        public string GetWelcomeMessage(Role role) => _messages[role];

        public IReadOnlyCollection<ModuleDescriptor> ListAvailableModules(Role role) => _availableModules[role];
    }
}
