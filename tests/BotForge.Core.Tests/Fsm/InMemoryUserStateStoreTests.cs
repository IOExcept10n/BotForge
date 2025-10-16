using BotForge.Fsm;
using BotForge.Messaging;
using System.Threading;
using Xunit;

namespace BotForge.Tests.Fsm;

public class InMemoryUserStateStoreTests
{
    [Fact]
    public async Task GetAndSaveState_Workflow()
    {
        var store = new InMemoryUserStateStore();
        var user = new UserIdentity(123);

        var state = await store.GetUserStateAsync(user);
        Assert.Equal(StateRecord.StartState, state);

        var result = new StateResult("next", "{\"x\":1}");
        await store.SaveAsync(user, result);

        var saved = await store.GetUserStateAsync(user);
        Assert.Equal("next", saved.Id);
        Assert.Equal("{\"x\":1}", saved.StateData);
    }
}
