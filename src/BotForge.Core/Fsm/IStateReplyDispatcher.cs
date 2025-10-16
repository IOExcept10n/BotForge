namespace BotForge.Core.Fsm;
public interface IStateReplyDispatcher
{
    Task SendAsync(StateResult result, StateContext ctx, CancellationToken cancellationToken = default);
}
