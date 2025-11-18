namespace BotForge.Modules.Attributes;

/// <summary>
/// Abstract attribute class for prompt states, inheriting from <see cref="FsmStateAttribute"/>.
/// </summary>
/// <param name="inputType">The type of input expected for the prompt state.</param>
/// <param name="messageResourceKey">The resource key for messages associated with the prompt state.</param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public abstract class PromptStateAttribute(Type inputType, string messageResourceKey) : FsmStateAttribute(messageResourceKey)
{
    /// <summary>
    /// Gets or sets a value indicating whether file input is allowed.
    /// </summary>
    public bool AllowFileInput { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether text input is allowed. Defaults to <see langword="true"/>.
    /// </summary>
    public bool AllowTextInput { get; init; } = true;

    /// <summary>
    /// Gets the expected input type for the prompt.
    /// </summary>
    public Type InputType { get; } = inputType;

    /// <summary>
    /// Gets or sets a value indicating whether the back button is enabled. Defaults to <see langword="true"/>.
    /// </summary>
    public bool BackButton { get; init; } = true;
}

/// <summary>
/// Attribute for prompt states with a generic input type.
/// </summary>
/// <typeparam name="T">The type of input expected for the prompt state.</typeparam>
/// <param name="messageResourceKey">The resource key for messages associated with the prompt state.</param>
[AttributeUsage(AttributeTargets.Method)]
public sealed class PromptStateAttribute<T>(string messageResourceKey) : PromptStateAttribute(typeof(T), messageResourceKey);
