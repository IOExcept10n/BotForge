namespace BotForge.Modules.Attributes
{
    /// <summary>
    /// An attribute to provide more information about a module.
    /// </summary>
    /// <param name="labelKey">A key for the button label that describes current module in main menu.</param>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ModuleAttribute(string labelKey) : Attribute
    {
        /// <summary>
        /// Gets a label key that represents the module in bot main menu.
        /// </summary>
        public string LabelKey { get; } = labelKey;

        /// <summary>
        /// Gets the name of the module, this will be used in state generation and resolving. Name should be unique for each module.
        /// </summary>
        public string? ModuleName { get; init; }

        /// <summary>
        /// Gets a list of the allowed role types that have access to this module. Each type should be assignable to <see cref="Roles.Role"/> and should have a parameterless constructor.
        /// </summary>
        /// <remarks>
        /// This property takes prededence over <see cref="AllowedRoleNames"/> when generating module descriptor.
        /// </remarks>
        public Type[]? AllowedRoleTypes { get; init; }

        /// <summary>
        /// Gets a list of the allowed role names that have access to this module.
        /// </summary>
        public string[]? AllowedRoleNames { get; init; }

        /// <summary>
        /// Gets a value indicating whether the module is displayed in main menu. If set to <see langword="false"/>, then module is accessible only manually via methods of group <see cref="ModuleBase.ToGlobalState{TModule}(Contexts.ModuleStateContext, string)"/>.
        /// </summary>
        public bool Display { get; init; } = true;

        /// <summary>
        /// Gets the module order in main menu. Modules are ordered by this number ascending.
        /// </summary>
        public int Order { get; init; }
    }
}
