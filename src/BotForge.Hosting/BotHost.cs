using BotForge.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BotForge.Hosting;

internal partial class BotHost(ITransport transport, UpdateProcessingPipeline pipeline, ILogger<BotHost> logger) : BackgroundService
{
    private readonly ITransport _transport = transport;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _transport.UpdateChannel.OnUpdate += async (sender, upd) => await Task.Run(async () => pipeline.HandleUpdateAsync(upd.Update), stoppingToken).ConfigureAwait(false);

        await _transport.StartAsync(stoppingToken).ConfigureAwait(false);

        await Task.Delay(Timeout.Infinite, stoppingToken).ConfigureAwait(false);
    }
}
