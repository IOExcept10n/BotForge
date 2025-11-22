using System.Reflection;
using BotForge.Fsm;
using BotForge.Modules.Attributes;
using BotForge.Modules.Contexts;

namespace BotForge.Modules.Handlers;

internal class PromptHandler<TModule, TData>(MethodInfo method, PromptAttribute<TData> attribute) : ModuleHandlerBase<TModule> where TModule : ModuleBase where TData : IParsable<TData>
{
    private readonly Func<TModule, PromptStateContext<TData>, StateResult> _expression = method.CreateDelegate<Func<TModule, PromptStateContext<TData>, StateResult>>();
    private readonly bool _allowTextInput = attribute.AllowTextInput;
    private readonly bool _allowFileInput = attribute.AllowFileInput;

    protected override async Task<StateResult> ExecuteInternalAsync(MessageStateContext ctx, CancellationToken cancellationToken = default)
    {
        using var module = CreateModule(ctx);

        var moduleContext = await GetModuleStateContextAsync(ctx, cancellationToken).ConfigureAwait(false);
        if (CheckCancel(moduleContext, module, out var back))
            return back;
        if (!moduleContext.TryToPromptContext<TData>(_allowTextInput, _allowFileInput, out var inputContext))
            return module.InvalidInput(moduleContext);

        return _expression(module, inputContext);
    }
}
