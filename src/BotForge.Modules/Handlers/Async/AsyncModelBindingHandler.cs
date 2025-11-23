using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using BotForge.Fsm;
using BotForge.Localization;
using BotForge.Modules.Contexts;
using Microsoft.Extensions.DependencyInjection;

namespace BotForge.Modules.Handlers.Async;

internal class AsyncModelBindingHandler<TModule, TModel>(MethodInfo method, IRegistry<ModelBindingDescriptor> modelRegistry) : ModuleHandlerBase<TModule> where TModule : ModuleBase where TModel : new()
{
    private readonly Func<TModule, ModelPromptContext<TModel>, Task<StateResult>> _expression = method.CreateDelegate<Func<TModule, ModelPromptContext<TModel>, Task<StateResult>>>();
    private readonly ModelBindingDescriptor _modelDescriptor = GetDescriptor(modelRegistry);

    protected override async Task<StateResult> ExecuteInternalAsync(MessageStateContext ctx, CancellationToken cancellationToken = default)
    {
        using var module = CreateModule(ctx);

        var moduleContext = await GetModuleStateContextAsync(ctx, cancellationToken).ConfigureAwait(false);
        var bindingContext = new ModelBindContext(
                    moduleContext.User,
                    moduleContext.Chat,
                    moduleContext.UserRole,
                    moduleContext.Message,
                    _modelDescriptor,
                    moduleContext.CurrentState,
                    moduleContext.Services);

        var result = await module.OnBindModelAsync(bindingContext).ConfigureAwait(false);
        if (result.HasError)
            return module.InvalidInput(moduleContext);

        if (result.Data.IsCompleted)
        {
            var modelContext = new ModelPromptContext<TModel>(
                moduleContext.User,
                moduleContext.Chat,
                moduleContext.UserRole,
                moduleContext.Message,
                result.Data.Model.Deserialize<TModel>()!,
                moduleContext.CurrentState,
                moduleContext.Services);

            return await _expression(module, modelContext).ConfigureAwait(false);
        }

        var localization = ctx.Services.GetRequiredService<ILocalizationService>();
        return ModuleBase.RetryWith(moduleContext, result.Data, localization.GetString(moduleContext.User.TargetLocale, _modelDescriptor.PropertyByName(result.Data.PropertyName)?.PromptKey ?? string.Empty));
    }

    private static ModelBindingDescriptor GetDescriptor(IRegistry<ModelBindingDescriptor> modelRegistry)
    {
        var modelType = typeof(TModel);
        if (modelRegistry.TryGet(modelType.FullName ?? modelType.Name, out var descriptor))
        {
            return descriptor;
        }

        descriptor = new(
                        typeof(TModel),
                        [..from p in modelType.GetProperties()
                           where p.CanWrite
                           let display = p.GetCustomAttribute<DisplayAttribute>()
                           select new ModelProperty(p.Name, display?.Prompt ?? p.Name, p)]);
        modelRegistry.Register(descriptor);
        return descriptor;
    }
}
