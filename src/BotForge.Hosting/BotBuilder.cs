using BotForge.Messaging;
using BotForge.Modules;
using BotForge.Modules.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.Metrics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BotForge.Hosting;

internal class BotBuilder : IBotBuilder
{
    private readonly HostApplicationBuilder _builder;

    public BotBuilder()
    {
        _builder = Host.CreateApplicationBuilder();
    }

    public BotBuilder(string[] args)
    {
        _builder = Host.CreateApplicationBuilder(args);
    }

    public BotBuilder(HostApplicationBuilderSettings settings)
    {
        _builder = Host.CreateApplicationBuilder(settings);
    }

    public IDictionary<object, object> Properties => ((IHostApplicationBuilder)_builder).Properties;

    public IConfigurationManager Configuration => _builder.Configuration;

    public IHostEnvironment Environment => _builder.Environment;

    public ILoggingBuilder Logging => _builder.Logging;

    public IMetricsBuilder Metrics => _builder.Metrics;

    public IServiceCollection Services => _builder.Services;

    public IHost Build()
    {
        _builder.Services.TryAddSingleton<UpdateProcessingPipeline>(p => new(p));
        ConfigureDefaultServices(_builder.Services);
        return _builder.Build();
    }

    public void ConfigureContainer<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory, Action<TContainerBuilder>? configure = null) where TContainerBuilder : notnull
    {
        _builder.ConfigureContainer(factory, configure);
    }

    private static void ConfigureDefaultServices(IServiceCollection services) =>
        services.TryConfigureCommands(builder => builder.AddCommand<StartCommandHandler>())
                .AddDefaultStorages()
                .AddDefaultUpdateHandlers()
                .AddFallbackModuleConfiguration()
                .AddFallbackRoleConfiguration()
                .AddDefaultRolesStorage()
                .AddHostedService<BotHost>();
}
