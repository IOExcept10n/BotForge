using BotForge.Fsm;
using BotForge.Messaging;
using BotForge.Modules.Layouts;

namespace BotForge.Modules.Roles;

internal sealed class SingleModuleMainMenuConfigurator(ILabelStore labels, IRegistry<StateDefinition> registry, IRegistry<State> stateHandlerRegistry) : IMainMenuConfigurator
{
    private readonly ILabelStore _labels = labels;
    private readonly IRegistry<StateDefinition> _registry = registry;
    private readonly IRegistry<State> _stateHandlerRegistry = stateHandlerRegistry;

    public void AddMainMenu(IRoleCatalog catalog, Role role)
    {
        var descriptor = catalog.ListAvailableModules(role).Single();
        if (!_registry.TryGet($"{descriptor.ModuleName}:{ModuleBase.RootStateName}", out var definition))
        {
            throw new InvalidOperationException("Cannot use single module main menu if the module is not registered.");
        }

        var keyboard = (definition.Layout as MenuStateLayout)?.Buttons;
        if (keyboard != null)
        {
            // Remove back button because it cannot navigate anyway — this state becomes root.
            keyboard = new(from row in keyboard.Buttons select row.Except([_labels.BackButton]));
        }
        definition = new StateDefinition(StateRecord.StartStateId, StateRecord.StartStateId, role.Name, new MenuStateLayout()
        {
            MessageKey = catalog.GetWelcomeMessage(role),
            Buttons = keyboard,
        });
        _registry.Register(definition);
        var state = new State(definition, new MainMenuHandler(descriptor));
        _stateHandlerRegistry.Register(state);
    }

    private class MainMenuHandler(ModuleDescriptor descriptor) : IStateHandler
    {
        public async Task<StateResult> ExecuteAsync(MessageStateContext ctx, CancellationToken cancellationToken = default) => await descriptor.RootState.ExecuteAsync(ctx, cancellationToken).ConfigureAwait(false);
    }
}
