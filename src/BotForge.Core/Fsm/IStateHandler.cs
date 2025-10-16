namespace BotForge.Core.Fsm;

public interface IStateHandler
{
    Task<StateResult> ExecuteAsync(MessageStateContext ctx, CancellationToken cancellationToken = default);
}
