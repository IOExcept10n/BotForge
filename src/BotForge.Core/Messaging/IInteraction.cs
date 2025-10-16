namespace BotForge.Core.Messaging;

public interface IInteraction
{
    UserIdentity From { get; }

    InteractionType Type { get; }

    string? CommandName { get; }

    IReadOnlyDictionary<string, string>? Options { get; }

    string? Query { get; }

    object? Raw { get; }
}

public enum InteractionType
{
    Command,
    CallbackQuery,
    Reaction,
}
