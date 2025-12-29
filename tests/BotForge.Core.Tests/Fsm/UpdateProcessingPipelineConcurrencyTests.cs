using BotForge.Fsm.Handling;
using BotForge.Messaging;
using BotForge.Middleware;
using Moq;

namespace BotForge.Tests.Fsm;

public sealed class UpdateProcessingPipelineConcurrencyTests
{
    [Fact]
    public async Task HandlesConcurrentUpdatesForSameUser()
    {
        // Arrange: create an engine with a raw update handler mock so FSM will forward to it
        var rawHandlerMock = new Mock<IRawUpdateHandler>();
        rawHandlerMock.Setup(h => h.HandleAsync(It.IsAny<IUpdate>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        using var pipeline = new UpdateProcessingPipeline(new ServiceProviderStub(rawHandlerMock.Object));

        var tasks = new List<Task>();
        for (int i = 0; i < 20; i++)
        {
            var upd = new TestUpdate(42);
            tasks.Add(pipeline.HandleUpdateAsync(upd));
        }

        // Act
        await Task.WhenAll(tasks);

        // Assert
        rawHandlerMock.Verify(h => h.HandleAsync(It.IsAny<IUpdate>(), It.IsAny<CancellationToken>()), Times.Exactly(20));
    }

    [Fact]
    public async Task SerializesPerUserConcurrency()
    {
        // Arrange
        var messageHandlerMock = new Mock<IMessageHandler>();
        var interactionHandlerMock = new Mock<IInteractionHandler>();
        var rawHandlerMock = new Mock<IRawUpdateHandler>();

        using var pipeline = new UpdateProcessingPipeline(new ServiceProviderStub(rawHandlerMock.Object), b => b.WithMaxUsers(100));

        var userId = 123L;
        var concurrentCalls = 10;
        var startBarrier = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        messageHandlerMock
            .Setup(m => m.HandleMessageAsync(It.IsAny<IMessage>(), It.IsAny<CancellationToken>()))
            .Returns(async () =>
            {
                // wait until all tasks are started
                await startBarrier.Task.ConfigureAwait(false);
                await Task.Delay(10).ConfigureAwait(false);
            });

        // Act: schedule many updates for the same user
        var tasks = new List<Task>();
        for (int i = 0; i < concurrentCalls; i++)
        {
            var update = new TestUpdate(userId);
            tasks.Add(pipeline.HandleUpdateAsync(update, CancellationToken.None));
        }

        // release handlers
        startBarrier.SetResult(true);
        await Task.WhenAll(tasks);

        // Assert: all tasks completed
        Assert.All(tasks, t => Assert.True(t.IsCompletedSuccessfully));
    }

    private sealed record TestUpdate(long UserId) : IUpdate
    {
        public DateTimeOffset Timestamp => DateTimeOffset.UtcNow;
        public UpdateType Type => UpdateType.MessageCreated;
        public UserIdentity Sender => new(UserId);
        public IMessage? Message => new TestMessage(new UserIdentity(UserId), new TextMessageContent("hi"));
        public IInteraction? Interaction => null;
        public object? RawUpdate => null;
    }

    private sealed class TestMessage : IMessage
    {
        public UserIdentity From { get; }
        public ChatId ChatId { get; }
        public MessageContent Content { get; }

        public TestMessage(UserIdentity from, MessageContent content)
        { From = from; Content = content; ChatId = new ChatId(1); }
    }

    private sealed class ServiceProviderStub : IServiceProvider
    {
        private readonly object? _svc;

        public ServiceProviderStub(object? svc) => _svc = svc;

        public object? GetService(Type serviceType) => serviceType == _svc?.GetType() ? _svc : null;
    }
}
