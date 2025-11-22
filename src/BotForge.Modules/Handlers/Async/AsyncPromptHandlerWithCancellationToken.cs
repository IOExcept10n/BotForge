using System.Reflection;
using BotForge.Fsm;
using BotForge.Modules.Attributes;
using BotForge.Modules.Contexts;

namespace BotForge.Modules.Handlers.Async;

internal class AsyncPromptHandlerWithCancellationToken<TModule, TData>(MethodInfo method, PromptAttribute<TData> attribute) : ModuleHandlerBase<TModule> where TModule : ModuleBase where TData : IParsable<TData>
{
    private readonly Func<TModule, PromptStateContext<TData>, CancellationToken, Task<StateResult>> _expression = method.CreateDelegate<Func<TModule, PromptStateContext<TData>, CancellationToken, Task<StateResult>>>();
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

        return await _expression(module, inputContext, cancellationToken).ConfigureAwait(false);
    }
}
