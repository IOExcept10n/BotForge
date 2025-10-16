using BotForge.Core.Fsm;
using Xunit;

namespace BotForge.Core.Tests.Fsm;

public class StateRegistryTests
{
    [Fact]
    public void RegisterAndGet_StateDefinition_IsRetrievable()
    {
        var registry = new StateRegistry();
        var layout = new TestLayout();
        var def = new StateDefinition("home", null, "cat", layout);

        registry.Register(def);

        Assert.True(registry.TryGet(def.StateId, out var fetched));
        Assert.Same(def, fetched);
    }

    private class TestLayout : IStateLayout
    {
        public Task SendLayoutMessageAsync(BotForge.Core.Messaging.IReplyChannel channel, BotForge.Core.Messaging.ChatId chatId, BotForge.Core.Messaging.ReplyContext? overrideMessage = null, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
