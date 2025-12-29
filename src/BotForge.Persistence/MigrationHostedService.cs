namespace BotForge.Persistence;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

internal sealed class MigrationHostedService(IServiceProvider services, PersistenceOptions options) : IHostedService
{
    private readonly IServiceProvider _services = services;
    private readonly PersistenceOptions _options = options;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_options.AutoMigrate && !_options.UseEnsureCreated)
        {
            return;
        }

        using var scope = _services.CreateScope();
        // Try to get BotForgeDbContext or any derived type
        var ctx = scope.ServiceProvider.GetService<BotForgeDbContext>() 
                  ?? scope.ServiceProvider.GetServices<DbContext>()
                      .OfType<BotForgeDbContext>()
                      .FirstOrDefault();
        if (ctx is null)
        {
            return;
        }

        if (_options.AutoMigrate)
        {
            try
            {
                // Try to apply migrations
                await ctx.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (InvalidOperationException)
            {
                // If migrations don't exist (e.g., user hasn't created any and default migrations aren't found),
                // fall back to EnsureCreated for the initial setup
                // This handles the case where the migrations assembly doesn't contain migrations
                if (_options.UseEnsureCreated)
                {
                    await ctx.Database.EnsureCreatedAsync(cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    // Re-throw if EnsureCreated is not enabled
                    throw;
                }
            }
        }
        else if (_options.UseEnsureCreated)
        {
            await ctx.Database.EnsureCreatedAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
