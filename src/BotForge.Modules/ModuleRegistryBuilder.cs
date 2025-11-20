using System.Collections;
using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using BotForge.Fsm;
using BotForge.Fsm.Handling;
using BotForge.Messaging;
using BotForge.Modules.Attributes;
using BotForge.Modules.Contexts;
using BotForge.Modules.Handlers;
using BotForge.Modules.Handlers.Async;
using BotForge.Modules.Layouts;
using BotForge.Modules.Roles;

namespace BotForge.Modules;

internal class ModuleRegistryBuilder(ILabelStore labelStore, IRegistry<StateDefinition> stateRegistry, IRegistry<ModelBindingDescriptor> bindingRegistry) : IModuleRegistryBuilder
{
    private readonly ILabelStore _labelStore = labelStore;
    private readonly IRegistry<StateDefinition> _stateRegistry = stateRegistry;
    private readonly IRegistry<ModelBindingDescriptor> _bindingRegistry = bindingRegistry;
    private readonly ModuleRegistry _registry = new();

    public IRegistry<ModuleDescriptor> Build() => _registry;

    public IModuleRegistryBuilder UseModule(Type moduleType, Action<ModuleDescriptor>? configure = null)
    {
        if (!moduleType.IsAssignableTo(moduleType))
            throw new ArgumentException("Requested type does not inherit from ModuleBase.", nameof(moduleType));

        var moduleAttribute = moduleType.GetCustomAttribute<ModuleAttribute>();

        string moduleName = moduleAttribute?.ModuleName ?? moduleType.Name;
        RoleSet roles = moduleAttribute switch
        {
            { AllowedRoleTypes: var types } when types is not null => new([..from t in types select (Role)Activator.CreateInstance(t)!]),
            { AllowedRoleNames: var names } when names is not null => new([..from n in names select new Role(n)]),
            _ => RoleSet.AllowAll,
        };

        var stateMethodCandidates = from method in moduleType.GetMethods()
                                    let stateAttr = method.GetCustomAttribute<FsmStateAttribute>()
                                    where stateAttr != null
                                    select (Method: method, State: stateAttr);

        Dictionary<string, IStateHandler> states = [];
        foreach (var candidate in stateMethodCandidates)
        {
            State state;

            // If the method is asynchronous, make async variation of handler.
            if (candidate.Method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                // If the last parameter is CancellationToken, make variations that accept it.
                if (candidate.Method.GetParameters()[^1].ParameterType == typeof(CancellationToken))
                {
                    state = candidate.State switch
                    {
                        MenuAttribute menu => Register(menu, candidate.Method, MakeMenuHandler(moduleType, typeof(AsyncMenuHandlerWithCancellationToken<>), candidate)),
                        PromptAttribute prompt => Register(prompt, candidate.Method, MakePromptHandler(moduleType, typeof(AsyncPromptHandlerWithCancellationToken<,>), candidate, prompt)),
                        ModelPromptAttribute modelPrompt => Register(modelPrompt, candidate.Method, MakeBindingHandler(moduleType, typeof(AsyncModelBindingHandlerWithCancellationToken<,>), candidate, modelPrompt)),
                        CustomStateAttribute custom => Register(custom, candidate.Method, MakeCustomHandler(moduleType, typeof(AsyncCustomHandlerWithCancellationToken<>), candidate)),
                        _ => throw new InvalidOperationException(
                            "Cannot register method because of unrecognizable state definition." +
                            "Write your own implementation of the IModuleRegistryBuilder to use custom state definitions."),
                    };
                }
                else
                {
                    state = candidate.State switch
                    {
                        MenuAttribute menu => Register(menu, candidate.Method, MakeMenuHandler(moduleType, typeof(AsyncMenuHandler<>), candidate)),
                        PromptAttribute prompt => Register(prompt, candidate.Method, MakePromptHandler(moduleType, typeof(AsyncPromptHandler<,>), candidate, prompt)),
                        ModelPromptAttribute modelPrompt => Register(modelPrompt, candidate.Method, MakeBindingHandler(moduleType, typeof(AsyncModelBindingHandler<,>), candidate, modelPrompt)),
                        CustomStateAttribute custom => Register(custom, candidate.Method, MakeCustomHandler(moduleType, typeof(AsyncCustomHandler<>), candidate)),
                        _ => throw new InvalidOperationException(
                            "Cannot register method because of unrecognizable state definition." +
                            "Write your own implementation of the IModuleRegistryBuilder to use custom state definitions."),
                    };
                }
            }
            else
            {
                state = candidate.State switch
                {
                    MenuAttribute menu => Register(menu, candidate.Method, MakeMenuHandler(moduleType, typeof(MenuHandler<>), candidate)),
                    PromptAttribute prompt => Register(prompt, candidate.Method, MakePromptHandler(moduleType, typeof(PromptHandler<,>), candidate, prompt)),
                    ModelPromptAttribute modelPrompt => Register(modelPrompt, candidate.Method, MakeBindingHandler(moduleType, typeof(ModelBindingHandler<,>), candidate, modelPrompt)),
                    CustomStateAttribute custom => Register(custom, candidate.Method, MakeCustomHandler(moduleType, typeof(CustomHandler<>), candidate)),
                    _ => throw new InvalidOperationException(
                        "Cannot register method because of unrecognizable state definition." +
                        "Write your own implementation of the IModuleRegistryBuilder to use custom state definitions."),
                };
            }

            states.Add(state.Definition.StateId, state.Handler);
        }

        var descriptor = new ModuleDescriptor(moduleName, moduleType, roles, states[$"{moduleName}:root"], states.ToFrozenDictionary());
        configure?.Invoke(descriptor);
        _registry.Register(descriptor);
        return this;
    }

    public IModuleRegistryBuilder ConfigureModule(Type moduleType, Action<ModuleDescriptor> configure)
    {
        if (!moduleType.IsAssignableTo(moduleType))
            throw new ArgumentException("Requested type does not inherit from ModuleBase.", nameof(moduleType));
        var moduleAttribute = moduleType.GetCustomAttribute<ModuleAttribute>();
        string moduleName = moduleAttribute?.ModuleName ?? moduleType.Name;
        if (!_registry.TryGet(moduleName, out var descriptor))
        {
            throw new InvalidOperationException("Couldn't configure module descriptor because it is not initialized. Call UseModule instead.");
        }
        configure(descriptor);
        return this;
    }

    private static IStateHandler MakeMenuHandler(Type moduleType, Type menuHandlerType, (MethodInfo Method, FsmStateAttribute State) candidate)
            => (IStateHandler)Activator.CreateInstance(menuHandlerType.MakeGenericType(moduleType), [candidate.Method])!;

    private static IStateHandler MakePromptHandler(Type moduleType, Type promptHandlerType, (MethodInfo Method, FsmStateAttribute State) candidate, PromptAttribute prompt)
        => (IStateHandler)Activator.CreateInstance(promptHandlerType.MakeGenericType(moduleType, prompt.InputType), [candidate.Method, prompt])!;

    private IStateHandler MakeBindingHandler(Type moduleType, Type bindingHandlerType, (MethodInfo Method, FsmStateAttribute State) candidate, ModelPromptAttribute modelPrompt)
        => (IStateHandler)Activator.CreateInstance(bindingHandlerType.MakeGenericType(moduleType, modelPrompt.InputType), [candidate.Method, _bindingRegistry])!;

    private static IStateHandler MakeCustomHandler(Type moduleType, Type handlerType, (MethodInfo Method, FsmStateAttribute State) candidate)
        => (IStateHandler)Activator.CreateInstance(handlerType.MakeGenericType(moduleType), [candidate.Method])!;

    private State Register(MenuAttribute menuAttribute, MethodInfo method, IStateHandler handler)
    {
        var buttons = from row in method.GetCustomAttributes<MenuRowAttribute>() select from item in row.LabelKeys select _labelStore.GetLabel(item);
        if (menuAttribute.BackButton)
            buttons = buttons.Append([_labelStore.BackButton]);
        
        var layout = new MenuStateLayout
        {
            MessageKey = menuAttribute.PromptLocalizationKey,
            Buttons = new(buttons),
            InheritKeyboard = method.GetCustomAttribute<InheritKeyboardAttribute>() != null,
            DisableKeyboard = method.GetCustomAttribute<RemoveKeyboardAttribute>() != null,
        };

        var definition = new StateDefinition(
            menuAttribute.StateName ?? method.Name,
            // Special case for root module state. It should refer to the main menu as parent.
            menuAttribute.StateName != ModuleBase.RootStateName ? $"{method.DeclaringType!.Name}:{menuAttribute.ParentStateName}" : "start",
            method.DeclaringType!.Name,
            layout);

        _stateRegistry.Register(definition);
        return new(definition, handler);
    }

    private State Register(PromptAttribute promptAttribute, MethodInfo method, IStateHandler handler)
    {
        var layout = new PromptStateLayout(_labelStore.CancelButton)
        {
            MessageKey = promptAttribute.PromptLocalizationKey,
            AllowCancel = promptAttribute.BackButton,
        };

        var definition = new StateDefinition(
            promptAttribute.StateName ?? method.Name,
            $"{method.DeclaringType!.Name}:{promptAttribute.ParentStateName}",
            method.DeclaringType!.Name,
            layout);

        _stateRegistry.Register(definition);
        return new(definition, handler);
    }

    private State Register(CustomStateAttribute stateAttribute, MethodInfo method, IStateHandler handler)
    {
        var layout = new PromptStateLayout(_labelStore.CancelButton)
        {
            MessageKey = stateAttribute.PromptLocalizationKey,
        };

        var definition = new StateDefinition(
            stateAttribute.StateName ?? method.Name,
            $"{method.DeclaringType!.Name}:{stateAttribute.ParentStateName}",
            method.DeclaringType!.Name,
            layout);

        _stateRegistry.Register(definition);
        return new(definition, handler);
    }

    private State Register(ModelPromptAttribute modelAttribute, MethodInfo method, IStateHandler handler)
    {
        _bindingRegistry.TryGet(modelAttribute.InputType.FullName ?? modelAttribute.InputType.Name, out var description);

        var layout = new PromptStateLayout(_labelStore.CancelButton)
        {
            MessageKey = description!.ModelProperties[0].PromptKey,
            AllowCancel = modelAttribute.BackButton,
        };

        var definition = new StateDefinition(
            modelAttribute.StateName ?? method.Name,
            $"{method.DeclaringType!.Name}:{modelAttribute.ParentStateName}",
            method.DeclaringType!.Name,
            layout);

        _stateRegistry.Register(definition);
        return new(definition, handler);
    }

    private readonly record struct State(StateDefinition Definition, IStateHandler Handler);

    private class ModuleRegistry : IRegistry<ModuleDescriptor>
    {
        private readonly Dictionary<string, ModuleDescriptor> _registry = [];

        public void Register(ModuleDescriptor instance) => _registry[instance.ModuleName] = instance;

        public bool TryGet(string key, [NotNullWhen(true)] out ModuleDescriptor? instance) => _registry.TryGetValue(key, out instance);

        public IEnumerator<ModuleDescriptor> GetEnumerator() => _registry.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
