using BotForge.Fsm;
using BotForge.Fsm.Handling;
using BotForge.Messaging;
using System.Threading;
using Xunit;

namespace BotForge.Tests.Fsm;

public class CommandRegistryTests
{
    [Fact]
    public void RegisterAndGetCommandHandlerIsRetrievable()
    {
        var registry = new CommandRegistry();
        var handler = new TestCommandHandler("hello");

        registry.Register(handler);

        Assert.True(registry.TryGet("hello", out var fetched));
        Assert.Same(handler, fetched);
    }

    private class TestCommandHandler : ICommandHandler
    {
        public string CommandName { get; }

        public TestCommandHandler(string name) => CommandName = name;

        public Task<StateResult> HandleCommand(InteractionStateContext ctx, CancellationToken cancellationToken = default) => Task.FromResult(new StateResult("next"));
    }
}
