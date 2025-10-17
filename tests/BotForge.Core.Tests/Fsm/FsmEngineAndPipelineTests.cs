using BotForge.Fsm;
using BotForge.Fsm.Handling;
using BotForge.Messaging;
using BotForge.Middleware;
using System.Threading;
using Xunit;

namespace BotForge.Tests.Fsm;

public class FsmEngineAndPipelineTests
{
    [Fact]
    public async Task FsmEngine_DispatchesToMessageHandler()
    {
        var called = false;
        var msgHandler = new FakeMessageHandler(() => called = true);

        var update = new TestUpdate(UpdateType.MessageCreated, new UserIdentity(1), new TestMessage(new UserIdentity(1), new TextMessageContent("x")));
        await FsmEngine.HandleAsync(update, new ServiceProviderStub(msgHandler), CancellationToken.None);

        Assert.True(called);
    }

    [Fact]
    public async Task UpdateProcessingPipeline_IgnoresNullSender()
    {
        var pipeline = new UpdateProcessingPipeline(new ServiceProviderStub(null));

        var update = new TestUpdate(UpdateType.MessageCreated, null, null);
        await pipeline.HandleUpdateAsync(update);

        // nothing to assert — just ensure no exception
    }

    private class FakeMessageHandler : IMessageHandler
    {
        private readonly Action _onCall;
        public FakeMessageHandler(Action onCall) => _onCall = onCall;
        public Task HandleMessageAsync(IMessage message, CancellationToken cancellationToken) { _onCall(); return Task.CompletedTask; }
    }

    private class TestUpdate : IUpdate
    {
        public TestUpdate(UpdateType type, UserIdentity? sender, IMessage? message)
        {
            Type = type; Sender = sender!; Message = message; Interaction = null; RawUpdate = null; Timestamp = DateTimeOffset.UtcNow;
        }
        public DateTimeOffset Timestamp { get; }
        public UpdateType Type { get; }
        public UserIdentity Sender { get; }
        public IMessage? Message { get; }
        public IInteraction? Interaction { get; }
        public object? RawUpdate { get; }
    }

    private class TestMessage : IMessage
    {
        public TestMessage(UserIdentity from, MessageContent content) { From = from; Content = content; ChatId = new ChatId(1); }
        public UserIdentity From { get; }
        public ChatId ChatId { get; }
        public MessageContent Content { get; }
    }

    private class ServiceProviderStub : IServiceProvider
    {
        private readonly object? _svc;
        public ServiceProviderStub(object? svc) => _svc = svc;
        public object? GetService(Type serviceType) => serviceType == _svc?.GetType() ? _svc : null;
    }
}
