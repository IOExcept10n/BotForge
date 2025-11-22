using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using BotForge.Messaging;
using BotForge.Modules.Roles;

namespace BotForge.Modules.Contexts;

/// <summary>
/// Represents a builder for model binding, facilitating the binding process with validation.
/// </summary>
/// <param name="Descriptor">The binding descriptor containing metadata about the model.</param>
/// <param name="InputProperty">The property currently being bound.</param>
/// <param name="Validation">The validation context for the binding.</param>
/// <param name="Model">The model instance being bound to.</param>
public record ModelBindingBuilder(ModelBindingDescriptor Descriptor, ModelProperty InputProperty, ValidationContext Validation, object Model)
{
    /// <summary>
    /// Creates a new instance of <see cref="ModelBindingBuilder"/> from the provided data.
    /// </summary>
    /// <param name="binding">The binding descriptor.</param>
    /// <param name="data">The model builder data.</param>
    /// <param name="services">The service provider for dependency injection.</param>
    /// <returns>A new instance of <see cref="ModelBindingBuilder"/>.</returns>
    public static ModelBindingBuilder FromData(ModelBindingDescriptor binding, ModelBuilderData data, IServiceProvider services)
    {
        ArgumentNullException.ThrowIfNull(binding);
        var model = data.Model.Deserialize(binding.RequestedModelType) ?? throw new InvalidOperationException();
        var property = binding.PropertyByName(data.PropertyName) ?? throw new InvalidOperationException();
        return new(
            binding,
            property,
            new(model, services, null)
            {
                MemberName = property.Name,
                DisplayName = property.PromptKey,
            },
            model);
    }

    /// <summary>
    /// Appends a value to the model property and validates it.
    /// </summary>
    /// <param name="propertyValue">The value to append to the property.</param>
    /// <returns>A result indicating whether there was an error during binding and validation.</returns>
    public BindingResult AppendValue(object? propertyValue)
    {
        try
        {
            List<ValidationResult> validationResults = [];
            if (!Validator.TryValidateProperty(propertyValue, Validation, validationResults))
            {
                return new(true, validationResults);
            }
            InputProperty.Property.SetValue(Model, propertyValue);
            JsonElement element = ToJsonElement(Model);
            var nextProperty = Descriptor.NextProperty(InputProperty);
            ModelBuilderData data = new(Descriptor.RequestedModelType.FullName!, nextProperty?.Name, ToJsonElement(Model), nextProperty == null);
            return new(false, validationResults, data);
        }
        catch (Exception ex)
        {
            return new(true, [new(ex.Message)]);
        }
    }

    /// <summary>
    /// Builds and retrieves the model instance of type T.
    /// </summary>
    /// <typeparam name="T">The type to which the model is being converted.</typeparam>
    /// <returns>The model instance of type T.</returns>
    /// <exception cref="ArgumentException">Thrown when the model cannot be converted to the requested type.</exception>
    public T Build<T>()
    {
        if (!typeof(T).IsAssignableTo(Descriptor.RequestedModelType))
            throw new ArgumentException($"Model cannot be converted to requested type. Expected type: {Descriptor.RequestedModelType}.", typeof(T).Name);
        return (T)Model;
    }

    /// <summary>
    /// Represents the result of a binding operation, indicating success or failure.
    /// </summary>
    /// <param name="HasError">Indicates whether there was an error during binding.</param>
    /// <param name="ValidationErrors">A collection of validation errors, if any.</param>
    /// <param name="Data">Additional data related to the binding operation.</param>
    public readonly record struct BindingResult(bool HasError, ICollection<ValidationResult> ValidationErrors, ModelBuilderData Data = default);

    /// <summary>
    /// Converts an object to a JSON element.
    /// </summary>
    /// <param name="obj">The object to convert.</param>
    /// <returns>A <see cref="JsonElement"/> representing the serialized object.</returns>
    public static JsonElement ToJsonElement(object? obj)
    {
        if (obj is null)
        {
            using var nullDoc = JsonDocument.Parse("null");
            return nullDoc.RootElement.Clone();
        }

        var bytes = JsonSerializer.SerializeToUtf8Bytes(obj);
        using var doc = JsonDocument.Parse(bytes);
        return doc.RootElement.Clone();
    }
}
