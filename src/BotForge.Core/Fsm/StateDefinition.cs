namespace BotForge.Fsm;

/// <summary>
/// Defines a state available in the FSM, including its name, optional parent and category,
/// and the <see cref="IStateLayout"/> used to render prompts or UI for the state.
/// </summary>
/// <param name="Name">The local name of the state (not including category).</param>
/// <param name="ParentStateId">Optional identifier of the parent state for hierarchical flows.</param>
/// <param name="Category">Optional category used to namespace states. When provided, it prefixes the <see cref="StateId"/>.</param>
/// <param name="Layout">The <see cref="IStateLayout"/> responsible for sending the state's layout message.</param>
public record StateDefinition(string Name, string? ParentStateId, string? Category, IStateLayout Layout)
{
    /// <summary>
    /// Gets the full state identifier. If <see cref="Category"/> is set, the format is "{Category}:{Name}"; otherwise it's just <see cref="Name"/>.
    /// </summary>
    public string StateId => Category != null ? $"{Category}:{Name}" : Name;
}
