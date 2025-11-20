namespace BotForge.Modules.Attributes
{
    public sealed class ModuleAttribute(string labelKey) : Attribute
    {
        public string? ModuleName { get; set; }

        public Type[]? AllowedRoleTypes { get; set; }

        public string[]? AllowedRoleNames { get; set; }
    }
}
