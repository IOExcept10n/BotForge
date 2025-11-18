namespace BotForge.Modules.Attributes;

/// <summary>
/// Base class for attributes representing button rows in a menu.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1813:Avoid unsealed attributes", Justification = "This class is inherited by MenuItemAttribute.")]
public class MenuRowAttribute(params string[] labelKeys) : Attribute
{
    /// <summary>
    /// Gets the label keys associated with the menu row.
    /// </summary>
    public string[] LabelKeys { get; } = labelKeys;
}
