using BotForge.Messaging;

namespace BotForge.Telegram.Integration.Models;

/// <summary>
/// DTO wrapper implementing <see cref="IInteraction"/>. Constructed by <see cref="TelegramUpdateParser"/>.
/// </summary>
public sealed class TelegramInteraction(UserIdentity from, InteractionType type, string? commandName, IReadOnlyDictionary<string, string>? options, string? query, object? raw) : IInteraction
{
    /// <inheritdoc/>
    public UserIdentity From { get; } = from;

    /// <inheritdoc/>
    public InteractionType Type { get; } = type;

    /// <inheritdoc/>
    public string? CommandName { get; } = commandName;

    /// <inheritdoc/>
    public IReadOnlyDictionary<string, string>? Options { get; } = options;

    /// <inheritdoc/>
    public string? Query { get; } = query;

    /// <inheritdoc/>
    public object? Raw { get; } = raw;
}
