namespace BotForge.Modules.Attributes;

/// <summary>
/// Attribute for menu states in FSM, inheriting from <see cref="FsmStateAttribute"/>.
/// </summary>
/// <param name="promptLocalizationKey">The resource key for prompt messages associated with the state.</param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class MenuAttribute(string promptLocalizationKey) : FsmStateAttribute(promptLocalizationKey)
{
    /// <summary>
    /// Gets or sets a value indicating whether the back button is enabled. Defaults to <see langword="true"/>.
    /// </summary>
    public bool BackButton { get; init; } = true;
}
