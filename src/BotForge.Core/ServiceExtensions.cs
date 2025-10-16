using BotForge.Fsm;
using BotForge.Fsm.Handling;
using BotForge.Localization;
using BotForge.Messaging;
using BotForge.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BotForge;

public static class ServiceExtensions
{
    public static IServiceCollection AddFsm(this IServiceCollection services)
    {
        services.TryAddScoped<IMessageHandler, MessageHandler>();
        services.TryAddScoped<IInteractionHandler, InteractionHandler>();
        services.AddSingleton<FsmEngine>();
        return services;
    }

    public static IServiceCollection AddFallbackStorages(this IServiceCollection services)
    {
        services.TryAddSingleton<IRegistry<ICommandHandler>, CommandRegistry>();
        services.TryAddSingleton<IRegistry<StateDefinition>, StateRegistry>();
        services.TryAddSingleton<ILabelStore, LabelStore>();
        services.TryAddSingleton<IUserStateStore, InMemoryUserStateStore>();
        services.TryAddSingleton<ILocalizationService, NoLocalizationService>();
        return services;
    }

    public static IServiceCollection ConfigureUpdatePipeline(this IServiceCollection services, Action<IUpdatePipelineBuilder> configure)
        => services.AddSingleton<UpdateProcessingPipeline>(p => new(p.GetRequiredService<FsmEngine>(), p, configure));
}
