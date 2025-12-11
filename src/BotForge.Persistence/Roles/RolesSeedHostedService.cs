namespace BotForge.Persistence.Roles;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using BotForge.Modules.Roles;
using BotForge.Persistence.Repositories;
using System.Linq;

internal sealed class RolesSeedHostedService(IServiceProvider services) : IHostedService
{
    private readonly IServiceProvider _services = services;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _services.CreateScope();
        var catalog = scope.ServiceProvider.GetService<IRoleCatalog>();
        var repo = scope.ServiceProvider.GetService<IBotRoleRepository>();
        if (catalog is null || repo is null)
            return;

        // Sequentially register roles to avoid concurrent DB conflicts
        foreach (var role in catalog.DefinedRoles.ToList())
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                await repo.RegisterAsync(role, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch
            {
                // ignore individual failures but continue seeding remaining roles
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
