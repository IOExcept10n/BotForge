using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json;
using BotForge.Fsm;
using BotForge.Messaging;
using BotForge.Modules.Roles;

namespace BotForge.Modules.Contexts;

/// <summary>
/// Represents the intermediate context for model binding, facilitating the binding of user input
/// to a model based on predefined descriptors.
/// </summary>
/// <remarks>
/// This context is handled internally, and you should not write custom handlers targeting such context.
/// However, for example, you can override method <see cref="ModuleBase.ValidateModelAsync(ModelBindContext, ModelBindingBuilder, ICollection{ValidationResult})"/> that relies on this context,
/// to customize model binding process.
/// </remarks>
/// <typeparam name="T">The type of model being bound.</typeparam>
/// <param name="User">The identity of the user.</param>
/// <param name="Chat">The chat identifier.</param>
/// <param name="Role">The role of the user.</param>
/// <param name="Message">The message that initiated state handling.</param>
/// <param name="Binding">The binding descriptor containing model metadata.</param>
/// <param name="CurrentState">The current state record of the context.</param>
/// <param name="Services">The service provider for dependency injection.</param>
public record ModelBindContext(
    UserIdentity User,
    ChatId Chat,
    Role Role,
    IMessage Message,
    ModelBindingDescriptor Binding,
    StateRecord CurrentState,
    IServiceProvider Services) :
    ModuleStateContext(User, Chat, Role, Message, CurrentState, Services);

/// <summary>
/// Represents the context for a model prompt, including a model that's been input by user.
/// </summary>
/// <typeparam name="TModel">The type of the model being manipulated.</typeparam>
/// <param name="User">The identity of the user.</param>
/// <param name="Chat">The chat identifier.</param>
/// <param name="Role">The role of the user.</param>
/// <param name="Message">The message that initiated state handling.</param>
/// <param name="Model">The model instance being worked on.</param>
/// <param name="CurrentState">The current state record of the context.</param>
/// <param name="Services">The service provider for dependency injection.</param>
public record ModelPromptContext<TModel>(
    UserIdentity User,
    ChatId Chat,
    Role Role,
    IMessage Message,
    TModel Model,
    StateRecord CurrentState,
    IServiceProvider Services) :
    ModuleStateContext(User, Chat, Role, Message, CurrentState, Services)
    where TModel : new();

/// <summary>
/// Represents a model property with metadata such as display name and reflection information.
/// </summary>
/// <param name="Name">The name of the property.</param>
/// <param name="PromptKey">The localization key for the display name of the property for user interfaces.</param>
/// <param name="Property">The reflection information for the associated property.</param>
public record ModelProperty(string Name, string PromptKey, PropertyInfo Property);

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

/// <summary>
/// Represents data about model building, including type information and completion status.
/// </summary>
/// <param name="TypeName">The name of the type being built.</param>
/// <param name="PropertyName">The name of the property currently being bound.</param>
/// <param name="Model">The JSON element representation of the model.</param>
/// <param name="IsCompleted">Indicates whether the model binding process is complete.</param>
public readonly record struct ModelBuilderData(string TypeName, string? PropertyName, JsonElement Model, bool IsCompleted);
