using System.Reflection;

namespace BotForge.Modules.Contexts;

/// <summary>
/// Represents a model property with metadata such as display name and reflection information.
/// </summary>
/// <param name="Name">The name of the property.</param>
/// <param name="PromptKey">The localization key for the display name of the property for user interfaces.</param>
/// <param name="Property">The reflection information for the associated property.</param>
public record ModelProperty(string Name, string PromptKey, PropertyInfo Property);
