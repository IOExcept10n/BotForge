using BotForge.Messaging;

namespace BotForge.Telegram.Integration.Models;

/// <summary>
/// Lightweight DTO that represents a BotForge `IUpdate` created from a Telegram update.
/// Parsing logic is provided by <see cref="TelegramUpdateParser"/>.
/// </summary>
public sealed class TelegramUpdate(DateTimeOffset timestamp, UpdateType type, UserIdentity sender, IMessage? message, IInteraction? interaction, object? rawUpdate) : IUpdate
{
    /// <inheritdoc/>
    public DateTimeOffset Timestamp { get; } = timestamp;

    /// <inheritdoc/>
    public UpdateType Type { get; } = type;

    /// <inheritdoc/>
    public UserIdentity Sender { get; } = sender;

    /// <inheritdoc/>
    public IMessage? Message { get; } = message;

    /// <inheritdoc/>
    public IInteraction? Interaction { get; } = interaction;

    /// <inheritdoc/>
    public object? RawUpdate { get; } = rawUpdate;
}
