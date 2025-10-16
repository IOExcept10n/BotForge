namespace BotForge.Messaging;

public interface IUpdateChannel
{
    event Func<IUpdate, CancellationToken, Task> OnUpdate;
}
