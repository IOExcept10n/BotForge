namespace BotForge.Modules.Attributes;

/// <summary>
/// Attribute to represent a single menu item.
/// </summary>
/// <param name="labelKey">The label key for the menu item.</param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class MenuItemAttribute(string labelKey) : MenuRowAttribute(labelKey)
{
    /// <summary>
    /// Gets the label key for the menu item.
    /// </summary>
    public string LabelKey { get; } = labelKey;
}
