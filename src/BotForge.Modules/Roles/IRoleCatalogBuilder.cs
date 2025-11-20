namespace BotForge.Modules.Roles;

public interface IRoleCatalogBuilder
{
    IRoleCatalogBuilder SetDefaultRole(Role role);

    IRoleCatalogBuilder AddRole(Role role, string welcomeMessageKey);

    IRoleCatalog Build();
}
