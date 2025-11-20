using System;
using System.Collections.Generic;
using System.Text;

namespace BotForge.Modules.Roles;

public static class RoleCatalogBuilderExtensions
{
    extension(IRoleCatalogBuilder builder)
    {
        public IRoleCatalogBuilder AddRole<T>(string welcomeMessageKey) where T : Role, new() => builder.AddRole(new T(), welcomeMessageKey);
    }
}
