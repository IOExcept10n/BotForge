namespace BotForge.Modules.Attributes;

/// <summary>
/// Abstract attribute class for model prompt states in FSM.
/// </summary>
/// <param name="inputType">The type of input expected for the model prompt.</param>
[AttributeUsage(AttributeTargets.Method)]
public abstract class ModelPromptStateAttribute(Type inputType) : FsmStateAttribute(string.Empty)
{
    /// <summary>
    /// Gets the expected input type for the model prompt.
    /// </summary>
    public Type InputType { get; } = inputType;

    /// <summary>
    /// Gets or sets a value indicating whether the back button is enabled. Defaults to <see langword="true"/>.
    /// </summary>
    public bool BackButton { get; init; } = true;
}

/// <summary>
/// Attribute for model prompt states with a generic input type.
/// </summary>
/// <typeparam name="T">The type of input expected for the model prompt.</typeparam>
[AttributeUsage(AttributeTargets.Method)]
public sealed class ModelPromptStateAttribute<T>() : ModelPromptStateAttribute(typeof(T)) where T : new();
