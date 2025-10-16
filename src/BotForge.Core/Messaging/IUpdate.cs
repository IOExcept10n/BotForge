using System.Diagnostics.CodeAnalysis;

namespace BotForge.Core.Messaging;

public interface IUpdate
{
    DateTimeOffset Timestamp { get; }

    UpdateType Type { get; }

    UserIdentity Sender { get; }

    IMessage? Message { get; }

    IInteraction? Interaction { get; }

    object? RawUpdate { get; }

    [MemberNotNullWhen(true, nameof(Message), nameof(RawUpdate))]
    bool IsMessage => Type is UpdateType.MessageCreated or UpdateType.MessageEdited or UpdateType.MessageDeleted;

    [MemberNotNullWhen(true, nameof(Interaction), nameof(RawUpdate))]
    bool IsInteraction => Type is UpdateType.Command or UpdateType.Interaction or UpdateType.ButtonClicked or UpdateType.CallbackQuery;
}

public enum UpdateType
{
    None,
    MessageCreated,
    MessageEdited,
    MessageDeleted,
    Command,
    Interaction,
    ButtonClicked,
    CallbackQuery,
    ReactionAdded,
    ReactionRemoved,
    MemberJoined,
    MemberLeft,
    System,
}
