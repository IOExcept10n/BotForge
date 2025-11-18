namespace BotForge.Modules.Attributes;

/// <summary>
/// Attribute to indicate that a method inherits keyboard settings.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class InheritKeyboardAttribute : Attribute;
