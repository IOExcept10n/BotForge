using System;

namespace BotForge.Modules;

/// <summary>
/// Defines a builder for registering modules in the BotForge framework.
/// </summary>
public interface IModuleRegistryBuilder
{
    /// <summary>
    /// Registers a module of a specified type and optionally configures its descriptor.
    /// </summary>
    /// <param name="moduleType">The <see cref="Type"/> of the module to be registered.</param>
    /// <param name="configure">An optional action to configure the <see cref="ModuleDescriptor"/> for the module.</param>
    /// <returns>The updated <see cref="IModuleRegistryBuilder"/> instance for chaining.</returns>
    IModuleRegistryBuilder UseModule(Type moduleType, Action<ModuleDescriptor>? configure = null);

    /// <summary>
    /// Configures a previously registered module's descriptor.
    /// </summary>
    /// <param name="moduleType">The <see cref="Type"/> of the module to configure.</param>
    /// <param name="configure">An action to configure the <see cref="ModuleDescriptor"/> for the module.</param>
    /// <returns>The updated <see cref="IModuleRegistryBuilder"/> instance for chaining.</returns>
    IModuleRegistryBuilder ConfigureModule(Type moduleType, Action<ModuleDescriptor> configure);

    /// <summary>
    /// Builds and returns the module registry, finalizing the registration of modules.
    /// </summary>
    /// <returns>
    /// An instance of <see cref="IRegistry{ModuleDescriptor}"/> containing all registered module descriptors.
    /// </returns>
    IRegistry<ModuleDescriptor> Build();
}
