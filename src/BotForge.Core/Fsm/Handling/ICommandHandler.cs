using BotForge.Core.Messaging;

namespace BotForge.Core.Fsm.Handling;
public interface ICommandHandler
{
    string CommandName { get; }

    public Task<StateResult> HandleCommand(InteractionStateContext ctx, CancellationToken cancellationToken = default);
}
