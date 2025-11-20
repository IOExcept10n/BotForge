using System.Reflection;

namespace BotForge.Modules;

public interface IModuleRegistryBuilder
{
    IModuleRegistryBuilder UseModule(Type moduleType, Action<ModuleDescriptor>? configure = null);

    IModuleRegistryBuilder ConfigureModule(Type moduleType, Action<ModuleDescriptor> configure);

    IRegistry<ModuleDescriptor> Build();
}
