using System.Reflection;

namespace BotForge.Modules;

/// <summary>
/// Provides extension methods for registering modules within a specified assembly in the BotForge framework.
/// </summary>
public static class ModuleRegistryBuilderExtensions
{
    /// <param name="builder">The <see cref="IModuleRegistryBuilder"/> instance to configure.</param>
    extension(IModuleRegistryBuilder builder)
    {
        /// <summary>
        /// Registers all modules found in the specified assembly that derive from <see cref="ModuleBase"/>.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/> containing the modules to register.</param>
        /// <returns>The updated <see cref="IModuleRegistryBuilder"/> instance for chaining.</returns>
        public IModuleRegistryBuilder UseAssembly(Assembly assembly)
        {
            ArgumentNullException.ThrowIfNull(assembly);
            foreach (var type in assembly.GetTypes().Where(x => x.IsAssignableTo(typeof(ModuleBase)) && !x.IsAbstract))
            {
                builder.UseModule(type);
            }
            return builder;
        }

        /// <summary>
        /// Registers all modules from the assembly of the specified type.
        /// </summary>
        /// <typeparam name="T">The type whose assembly is used to find modules.</typeparam>
        /// <returns>The updated <see cref="IModuleRegistryBuilder"/> instance for chaining.</returns>
        public IModuleRegistryBuilder UseAssemblyByType<T>() => builder.UseAssembly(typeof(T).Assembly);

        /// <summary>
        /// Registers a specific module type that derives from <see cref="ModuleBase"/>.
        /// </summary>
        /// <typeparam name="T">The module type to register.</typeparam>
        /// <returns>The updated <see cref="IModuleRegistryBuilder"/> instance for chaining.</returns>
        public IModuleRegistryBuilder UseModule<T>() where T : ModuleBase => builder.UseModule(typeof(T));
    }
}
