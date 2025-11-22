using BotForge.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BotForge.Hosting;

internal partial class BotHost(ITransport transport, UpdateProcessingPipeline pipeline, ILogger<BotHost> logger) : BackgroundService
{
    private readonly ITransport _transport = transport;
    private readonly ILogger<BotHost> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _transport.UpdateChannel.OnUpdate += async (sender, upd) => await Task.Run(async () => pipeline.HandleUpdateAsync(upd.Update), stoppingToken).ConfigureAwait(false);
            await _transport.StartAsync(stoppingToken).ConfigureAwait(false);
            if (_logger.IsEnabled(LogLevel.Information))
                Log_BotStarting(_logger, _transport.ClientName);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            Log_BotException(_logger, ex);
            throw;
        }

        // After bot initialization, just wait forever until application stops.
        await Task.Delay(Timeout.Infinite, stoppingToken).ConfigureAwait(false);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Bot @{BotName} is started listening.")]
    private static partial void Log_BotStarting(ILogger<BotHost> logger, string? botName);

    [LoggerMessage(Level = LogLevel.Critical, Message = "Unhandled fatal exception while starting bot.")]
    private static partial void Log_BotException(ILogger<BotHost> logger, Exception ex);
}
