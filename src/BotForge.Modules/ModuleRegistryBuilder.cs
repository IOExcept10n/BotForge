using System.Collections;
using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using BotForge.Fsm;
using BotForge.Messaging;
using BotForge.Modules.Attributes;
using BotForge.Modules.Contexts;
using BotForge.Modules.Handlers;
using BotForge.Modules.Handlers.Async;
using BotForge.Modules.Layouts;
using BotForge.Modules.Roles;

namespace BotForge.Modules;

internal class ModuleRegistryBuilder(ILabelStore labelStore, IRegistry<StateDefinition> stateRegistry, IRegistry<State> stateHandlerRegistry, IRegistry<ModelBindingDescriptor> bindingRegistry) : IModuleRegistryBuilder
{
    private const string ModuleSuffix = nameof(Module);

    private readonly ILabelStore _labelStore = labelStore;
    private readonly IRegistry<StateDefinition> _stateRegistry = stateRegistry;
    private readonly IRegistry<State> _stateHandlerRegistry = stateHandlerRegistry;
    private readonly IRegistry<ModelBindingDescriptor> _bindingRegistry = bindingRegistry;
    private readonly ModuleRegistry _registry = new();

    public IRegistry<ModuleDescriptor> Build() => _registry;

    public IModuleRegistryBuilder UseModule(Type moduleType, Action<ModuleDescriptor>? configure = null)
    {
        if (!moduleType.IsAssignableTo(moduleType))
            throw new ArgumentException("Requested type does not inherit from ModuleBase.", nameof(moduleType));

        var moduleAttribute = moduleType.GetCustomAttribute<ModuleAttribute>();

        string moduleName = moduleAttribute?.ModuleName ?? moduleType.Name;
        if (!string.Equals(moduleName, ModuleSuffix, StringComparison.OrdinalIgnoreCase) &&
            moduleName.EndsWith(ModuleSuffix, StringComparison.OrdinalIgnoreCase))
        {
            moduleName = moduleName[..^ModuleSuffix.Length];
        }
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
            if (candidate.Method.ReturnType.IsGenericType && candidate.Method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                // If the last parameter is CancellationToken, make variations that accept it.
                if (candidate.Method.GetParameters()[^1].ParameterType == typeof(CancellationToken))
                {
                    state = candidate.State switch
                    {
                        MenuAttribute menu when GetMenuAttributes(candidate.Method) is var attributes => Register(moduleName, menu, candidate.Method, MakeMenuHandler(moduleType, moduleName, typeof(AsyncMenuHandlerWithCancellationToken<>), candidate, attributes.OfType<MenuRowAttribute>()), attributes),
                        PromptAttribute prompt => Register(moduleName, prompt, candidate.Method, MakePromptHandler(moduleType, moduleName, typeof(AsyncPromptHandlerWithCancellationToken<,>), candidate, prompt)),
                        ModelPromptAttribute modelPrompt => Register(moduleName, modelPrompt, candidate.Method, MakeBindingHandler(moduleType, moduleName, typeof(AsyncModelBindingHandlerWithCancellationToken<,>), candidate, modelPrompt)),
                        CustomStateAttribute custom => Register(moduleName, custom, candidate.Method, MakeCustomHandler(moduleType, moduleName, typeof(AsyncCustomHandlerWithCancellationToken<>), candidate)),
                        _ => throw new InvalidOperationException(
                            "Cannot register method because of unrecognizable state definition." +
                            "Write your own implementation of the IModuleRegistryBuilder to use custom state definitions."),
                    };
                }
                else
                {
                    state = candidate.State switch
                    {
                        MenuAttribute menu when GetMenuAttributes(candidate.Method) is var attributes => Register(moduleName, menu, candidate.Method, MakeMenuHandler(moduleType, moduleName, typeof(AsyncMenuHandler<>), candidate, attributes.OfType<MenuRowAttribute>()), attributes),
                        PromptAttribute prompt => Register(moduleName, prompt, candidate.Method, MakePromptHandler(moduleType, moduleName, typeof(AsyncPromptHandler<,>), candidate, prompt)),
                        ModelPromptAttribute modelPrompt => Register(moduleName, modelPrompt, candidate.Method, MakeBindingHandler(moduleType, moduleName, typeof(AsyncModelBindingHandler<,>), candidate, modelPrompt)),
                        CustomStateAttribute custom => Register(moduleName, custom, candidate.Method, MakeCustomHandler(moduleType, moduleName, typeof(AsyncCustomHandler<>), candidate)),
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
                    MenuAttribute menu when GetMenuAttributes(candidate.Method) is var attributes => Register(moduleName, menu, candidate.Method, MakeMenuHandler(moduleType, moduleName, typeof(MenuHandler<>), candidate, attributes.OfType<MenuRowAttribute>()), attributes),
                    PromptAttribute prompt => Register(moduleName, prompt, candidate.Method, MakePromptHandler(moduleType, moduleName, typeof(PromptHandler<,>), candidate, prompt)),
                    ModelPromptAttribute modelPrompt => Register(moduleName, modelPrompt, candidate.Method, MakeBindingHandler(moduleType, moduleName, typeof(ModelBindingHandler<,>), candidate, modelPrompt)),
                    CustomStateAttribute custom => Register(moduleName, custom, candidate.Method, MakeCustomHandler(moduleType, moduleName, typeof(CustomHandler<>), candidate)),
                    _ => throw new InvalidOperationException(
                        "Cannot register method because of unrecognizable state definition." +
                        "Write your own implementation of the IModuleRegistryBuilder to use custom state definitions."),
                };
            }

            states.Add(state.Definition.StateId, state.Handler);
            _stateRegistry.Register(state.Definition);
            _stateHandlerRegistry.Register(state);
        }
        
        var descriptor = new ModuleDescriptor(moduleName, moduleType, _labelStore.GetLabel(moduleAttribute?.LabelKey ?? moduleName), roles, states[$"{moduleName}:root"], moduleAttribute?.Order ?? 0, moduleAttribute?.Display ?? true, states.ToFrozenDictionary());
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
        if (!string.Equals(moduleName, ModuleSuffix, StringComparison.OrdinalIgnoreCase) &&
            moduleName.EndsWith(ModuleSuffix, StringComparison.OrdinalIgnoreCase))
        {
            moduleName = moduleName[..^ModuleSuffix.Length];
        }
        if (!_registry.TryGet(moduleName, out var descriptor))
        {
            throw new InvalidOperationException("Couldn't configure module descriptor because it is not initialized. Call UseModule instead.");
        }
        configure(descriptor);
        return this;
    }

    private IStateHandler MakeMenuHandler(Type moduleType, string moduleName, Type menuHandlerType, (MethodInfo Method, FsmStateAttribute State) candidate, IEnumerable<MenuRowAttribute> rowAttributes)
    {
        var handler = (IModuleHandlerBase)Activator.CreateInstance(menuHandlerType.MakeGenericType(moduleType), [candidate.Method, _labelStore, rowAttributes])!;
        handler.ModuleName = moduleName;
        return handler;
    }

    private static IStateHandler MakePromptHandler(Type moduleType, string moduleName, Type promptHandlerType, (MethodInfo Method, FsmStateAttribute State) candidate, PromptAttribute prompt)
    {
        var handler = (IModuleHandlerBase)Activator.CreateInstance(promptHandlerType.MakeGenericType(moduleType, prompt.InputType), [candidate.Method, prompt])!;
        handler.ModuleName = moduleName;
        return handler;
    }

    private IStateHandler MakeBindingHandler(Type moduleType, string moduleName, Type bindingHandlerType, (MethodInfo Method, FsmStateAttribute State) candidate, ModelPromptAttribute modelPrompt)
    {
        var handler = (IModuleHandlerBase)Activator.CreateInstance(bindingHandlerType.MakeGenericType(moduleType, modelPrompt.InputType), [candidate.Method, _bindingRegistry])!;
        handler.ModuleName = moduleName;
        return handler;
    }

    private static IStateHandler MakeCustomHandler(Type moduleType, string moduleName, Type handlerType, (MethodInfo Method, FsmStateAttribute State) candidate)
    {
        var handler = (IModuleHandlerBase)Activator.CreateInstance(handlerType.MakeGenericType(moduleType), [candidate.Method])!;
        handler.ModuleName = moduleName;
        return handler;
    }

    private State Register(string moduleName, MenuAttribute menuAttribute, MethodInfo method, IStateHandler handler, List<Attribute> attributesList)
    {
        var buttons = from row in attributesList.OfType<MenuRowAttribute>() select from item in row.LabelKeys select _labelStore.GetLabel(item);
        if (menuAttribute.BackButton)
            buttons = buttons.Append([_labelStore.BackButton]);

        var layout = new MenuStateLayout
        {
            MessageKey = menuAttribute.PromptLocalizationKey,
            Buttons = new(buttons),
            InheritKeyboard = attributesList.OfType<InheritKeyboardAttribute>().Any(),
            DisableKeyboard = attributesList.OfType<RemoveKeyboardAttribute>().Any(),
        };

        var definition = new StateDefinition(
            menuAttribute.StateName ?? method.Name,
            // Special case for root module state. It should refer to the main menu as parent.
            menuAttribute.StateName != ModuleBase.RootStateName ? $"{moduleName}:{menuAttribute.ParentStateName}" : StateRecord.StartStateId,
            moduleName,
            layout);

        return new(definition, handler);
    }

    private static List<Attribute> GetMenuAttributes(MethodInfo method)
    {
        var methodAttributes = method.GetCustomAttributes<Attribute>(inherit: true);

        // Special case for root module state. It has a synchronous variation that has no separate state.
        // So we need to transfer all attributes from it to an asynchronous implementation.
        if (method.Name == nameof(ModuleBase.OnModuleRootAsync))
        {
            var syncMethod = method.ReflectedType!.GetMethod(nameof(ModuleBase.OnModuleRoot))!;
            methodAttributes = methodAttributes.Concat(syncMethod.GetCustomAttributes<Attribute>(inherit: true));
        }

        var attributesList = methodAttributes.ToList();
        return attributesList;
    }

    private State Register(string moduleName, PromptAttribute promptAttribute, MethodInfo method, IStateHandler handler)
    {
        var layout = new PromptStateLayout(_labelStore.CancelButton)
        {
            MessageKey = promptAttribute.PromptLocalizationKey,
            AllowCancel = promptAttribute.BackButton,
        };

        var definition = new StateDefinition(
            promptAttribute.StateName ?? method.Name,
            $"{moduleName}:{promptAttribute.ParentStateName}",
            moduleName,
            layout);

        return new(definition, handler);
    }

    private State Register(string moduleName, CustomStateAttribute stateAttribute, MethodInfo method, IStateHandler handler)
    {
        var layout = new PromptStateLayout(_labelStore.CancelButton)
        {
            MessageKey = stateAttribute.PromptLocalizationKey,
        };

        var definition = new StateDefinition(
            stateAttribute.StateName ?? method.Name,
            $"{moduleName}:{stateAttribute.ParentStateName}",
            moduleName,
            layout);

        return new(definition, handler);
    }

    private State Register(string moduleName, ModelPromptAttribute modelAttribute, MethodInfo method, IStateHandler handler)
    {
        _bindingRegistry.TryGet(modelAttribute.InputType.FullName ?? modelAttribute.InputType.Name, out var description);

        var layout = new PromptStateLayout(_labelStore.CancelButton)
        {
            MessageKey = description!.ModelProperties[0].PromptKey,
            AllowCancel = modelAttribute.BackButton,
        };

        var definition = new StateDefinition(
            modelAttribute.StateName ?? method.Name,
            $"{moduleName}:{modelAttribute.ParentStateName}",
            moduleName,
            layout);

        return new(definition, handler);
    }

    private class ModuleRegistry : IRegistry<ModuleDescriptor>
    {
        private readonly Dictionary<string, ModuleDescriptor> _registry = [];

        public void Register(ModuleDescriptor instance) => _registry[instance.ModuleName] = instance;

        public bool TryGet(string key, [NotNullWhen(true)] out ModuleDescriptor? instance) => _registry.TryGetValue(key, out instance);

        public IEnumerator<ModuleDescriptor> GetEnumerator() => _registry.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
