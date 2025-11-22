namespace BotForge.Modules.Contexts;

/// <summary>
/// Describes the details for model binding, including the properties involved.
/// </summary>
/// <param name="RequestedModelType">The type of the model being requested for binding.</param>
/// <param name="ModelProperties">The properties of the model that can be bound.</param>
public record ModelBindingDescriptor(Type RequestedModelType, ModelProperty[] ModelProperties)
{
    /// <summary>
    /// Gets the next property in the binding descriptor after the current property.
    /// </summary>
    /// <param name="current">The current model property.</param>
    /// <returns>The next model property if available; otherwise, null.</returns>
    public ModelProperty? NextProperty(ModelProperty current)
    {
        int index = Array.IndexOf(ModelProperties, current);
        if (index == ModelProperties.Length - 1)
            return null;
        if (index < 0)
            return ModelProperties[0];
        return ModelProperties[index + 1];
    }

    /// <summary>
    /// Finds a property by its name within the model properties.
    /// </summary>
    /// <param name="propertyName">The name of the property to find.</param>
    /// <returns>The associated model property if found; otherwise, null.</returns>
    public ModelProperty? PropertyByName(string? propertyName) => Array.Find(ModelProperties, x => x.Name == propertyName);
}
