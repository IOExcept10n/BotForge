namespace BotForge.Modules.Attributes;

/// <summary>
/// Attribute to indicate that a method should remove any associated keyboard settings.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class RemoveKeyboardAttribute : Attribute;
