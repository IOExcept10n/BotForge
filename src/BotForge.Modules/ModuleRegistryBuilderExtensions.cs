using System.Reflection;

namespace BotForge.Modules;

public static class ModuleRegistryBuilderExtensions
{
    extension(IModuleRegistryBuilder builder)
    {
        public IModuleRegistryBuilder UseAssembly(Assembly assembly)
        {
            ArgumentNullException.ThrowIfNull(assembly);
            foreach (var type in assembly.GetTypes().Where(x => x.IsAssignableTo(typeof(ModuleBase)) && !x.IsAbstract))
            {
                builder.UseModule(type);
            }
            return builder;
        }

        public IModuleRegistryBuilder UseAssemblyByType<T>() => builder.UseAssembly(typeof(T).Assembly);

        public IModuleRegistryBuilder UseModule<T>() where T : ModuleBase => builder.UseModule(typeof(T));
    }
}
