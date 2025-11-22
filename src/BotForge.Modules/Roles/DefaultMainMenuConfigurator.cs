using System.Globalization;
using BotForge.Fsm;
using BotForge.Localization;
using BotForge.Messaging;
using BotForge.Modules.Layouts;
using Microsoft.Extensions.DependencyInjection;

namespace BotForge.Modules.Roles;

internal class DefaultMainMenuConfigurator(IRegistry<StateDefinition> registry, IRegistry<State> stateHandlerRegistry) : IMainMenuConfigurator
{
    private readonly IRegistry<StateDefinition> _registry = registry;
    private readonly IRegistry<State> _stateHandlerRegistry = stateHandlerRegistry;

    public void AddMainMenu(IRoleCatalog catalog, Role role)
    {
        var descriptors = catalog.ListAvailableModules(role);
        var definition = new StateDefinition(StateRecord.StartStateId, StateRecord.StartStateId, role.Name, new MenuStateLayout()
        {
            MessageKey = catalog.GetWelcomeMessage(role),
            Buttons = new([.. from module in descriptors select (IEnumerable<ButtonLabel>)[module.ModuleButton]])
        });
        _registry.Register(definition);
        var state = new State(definition, new MainMenuHandler(descriptors));
        _stateHandlerRegistry.Register(state);
    }

    private class MainMenuHandler(IReadOnlyCollection<ModuleDescriptor> modules) : IStateHandler
    {
        public Task<StateResult> ExecuteAsync(MessageStateContext ctx, CancellationToken cancellationToken = default)
        {
            var module = modules.FirstOrDefault(m => ctx.Matches(m.ModuleButton));
            if (module == null)
            {
                string invalidInputMessage = ctx.Services.GetRequiredService<ILocalizationService>().GetString(ctx.Message.From.Locale ?? CultureInfo.InvariantCulture, ModuleBase.InvalidInputKey);
                return Task.FromResult(new StateResult(ctx.CurrentState.Id, ModuleBase.InvalidInputKey, new(invalidInputMessage, null)));
            }
            return Task.FromResult(new StateResult($"{module.ModuleName}:{ModuleBase.RootStateName}"));
        }
    }
}
