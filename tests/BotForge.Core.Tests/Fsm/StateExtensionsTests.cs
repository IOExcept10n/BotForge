using BotForge.Core.Fsm;
using BotForge.Core.Messaging;
using BotForge.Core.Localization;
using Xunit;

namespace BotForge.Core.Tests.Fsm;

public class StateExtensionsTests
{
    [Fact]
    public void TryGetData_ReturnsFalseOnNullOrInvalidJson()
    {
        StateRecord rec = new StateRecord("s", "not-json");
        var ctx = new StateContext(rec, new ServiceProviderStub(new NoLocalizationService()));

        Assert.False(StateExtensions.TryGetData<int>(null!, out _));
        Assert.False(StateExtensions.TryGetData<int>(ctx, out _));
    }

    [Fact]
    public void WithData_SerializesData()
    {
        var res = new StateResult("s", string.Empty);
        var newRes = StateExtensions.WithData(res, new { A = 1 });

        Assert.Contains("\"A\":1", newRes.NextStateData);
    }

    [Fact]
    public void MessageMatches_LabelMatchesText()
    {
        var svc = new NoLocalizationService();
        var label = new ButtonLabel(Emoji.None, "hi");
        var msg = new TestMessage(new UserIdentity(1), new TextMessageContent("hi"));
        var state = StateRecord.StartState;
        var ctx = new MessageStateContext(msg, state, new ServiceProviderStub(svc));

        Assert.True(StateExtensions.Matches(ctx, label));
    }

    private class TestMessage : IMessage
    {
        public UserIdentity From { get; }
        public ChatId ChatId { get; } = new(1);
        public MessageContent Content { get; }
        public TestMessage(UserIdentity from, MessageContent content) { From = from; Content = content; }
    }

    private class ServiceProviderStub : IServiceProvider
    {
        private readonly ILocalizationService _svc;
        public ServiceProviderStub(ILocalizationService svc) => _svc = svc;
        public object? GetService(Type serviceType) => _svc;
    }
}
