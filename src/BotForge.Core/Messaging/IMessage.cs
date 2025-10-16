namespace BotForge.Messaging;

public interface IMessage
{
    UserIdentity From { get; }

    ChatId ChatId { get; }

    MessageContent Content { get; }
}

public readonly record struct ChatId(long Id)
{
    public static implicit operator ChatId(long id) => new(id);

    public static ChatId FromInt64(long id) => new(id);
}
