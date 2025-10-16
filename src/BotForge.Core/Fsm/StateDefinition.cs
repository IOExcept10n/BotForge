namespace BotForge.Fsm;

public record StateDefinition(string Name, string? ParentStateId, string? Category, IStateLayout Layout)
{
    public string StateId => Category != null ? $"{Category}:{Name}" : Name;
}
