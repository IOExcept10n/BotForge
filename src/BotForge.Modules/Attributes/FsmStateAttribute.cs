namespace BotForge.Modules.Attributes;

/// <summary>
/// Base class for attributes representing finite state machine (FSM) states.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public abstract class FsmStateAttribute(string messageResourceKey) : Attribute
{
    /// <summary>
    /// Gets or sets the name of the state.
    /// </summary>
    public string? StateName { get; init; }

    /// <summary>
    /// Gets or sets the name of the parent state. Defaults to "root".
    /// </summary>
    public string? ParentStateName { get; init; } = "root";

    /// <summary>
    /// Gets the resource key for messages associated with the state.
    /// </summary>
    public string MessageResourceKey { get; } = messageResourceKey;
}
