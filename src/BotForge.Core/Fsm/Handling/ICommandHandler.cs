namespace BotForge.Fsm.Handling;

public interface ICommandHandler
{
    string CommandName { get; }

    public Task<StateResult> HandleCommand(InteractionStateContext ctx, CancellationToken cancellationToken = default);
}
