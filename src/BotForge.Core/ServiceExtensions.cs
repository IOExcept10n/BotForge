using System.Runtime.CompilerServices;
using BotForge.Fsm;
using BotForge.Fsm.Handling;
using BotForge.Localization;
using BotForge.Messaging;
using BotForge.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BotForge;

/// <summary>
/// Contains dependency injection extension methods for registering FSM and related services
/// used by the chatbot library.
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Registers the finite-state-machine (FSM) update handler services:
    /// <list type="bullet">
    /// <item><see cref="IMessageHandler"/> (scoped)</item>
    /// <item><see cref="IInteractionHandler"/> (scoped)</item>
    /// </list>
    /// Use this method to add default handlers to the service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The original <see cref="IServiceCollection"/> for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IServiceCollection AddDefaultUpdateHandlers(this IServiceCollection services)
    {
        services.TryAddScoped<IMessageHandler, MessageHandler>();
        services.TryAddScoped<IInteractionHandler, InteractionHandler>();
        return services;
    }

    /// <summary>
    /// Registers fallback/default implementations for various storage and registry services:
    /// <list type="bullet">
    /// <item><see cref="IRegistry{ICommandHandler}"/> -> <see cref="CommandRegistry"/></item>
    /// <item><see cref="IRegistry{StateDefinition}"/> -> <see cref="StateRegistry"/></item>
    /// <item><see cref="ILabelStore"/> -> <see cref="LabelStore"/></item>
    /// <item><see cref="IUserStateStore"/> -> <see cref="InMemoryUserStateStore"/></item>
    /// <item><see cref="ILocalizationService"/> -> <see cref="NoLocalizationService"/></item>
    /// </list>
    /// These are registered as singletons and can be replaced by callers prior to building the provider.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The original <see cref="IServiceCollection"/> for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IServiceCollection AddDefaultStorages(this IServiceCollection services)
    {
        services.TryAddSingleton<IRegistry<ICommandHandler>, CommandRegistry>();
        services.TryAddSingleton<IRegistry<StateDefinition>, StateRegistry>();
        services.TryAddSingleton<ILabelStore, LabelStore>();
        services.TryAddSingleton<IUserStateStore, InMemoryUserStateStore>();
        services.TryAddSingleton<ILocalizationService, NoLocalizationService>();
        return services;
    }

    /// <summary>
    /// Configures and registers the update processing pipeline as a singleton.
    /// The provided <paramref name="configure"/> action is invoked with an
    /// <see cref="IUpdatePipelineBuilder"/> when constructing the pipeline.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configure">
    /// An action that configures the <see cref="IUpdatePipelineBuilder"/> used to build the
    /// <see cref="UpdateProcessingPipeline"/>.
    /// </param>
    /// <returns>The original <see cref="IServiceCollection"/> for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IServiceCollection ConfigureUpdatePipeline(this IServiceCollection services, Action<IUpdatePipelineBuilder> configure)
        => services.AddSingleton<UpdateProcessingPipeline>(p => new(p, configure));
}
