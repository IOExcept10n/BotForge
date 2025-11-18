namespace BotForge.Modules.Attributes;

/// <summary>
/// Attribute for menu states in FSM, inheriting from <see cref="FsmStateAttribute"/>.
/// </summary>
/// <param name="messageResourceKey">The resource key for messages associated with the menu state.</param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class MenuStateAttribute(string messageResourceKey) : FsmStateAttribute(messageResourceKey)
{
    /// <summary>
    /// Gets or sets a value indicating whether the back button is enabled. Defaults to true.
    /// </summary>
    public bool BackButton { get; init; } = true;
}
