using System.Reflection;
using BotForge.Fsm;
using BotForge.Modules.Contexts;

namespace BotForge.Modules.Handlers.Async;

internal sealed class AsyncCustomHandler<TModule>(MethodInfo method) : ModuleHandlerBase<TModule> where TModule : ModuleBase
{
    private readonly Func<TModule, ModuleStateContext, Task<StateResult>> _expression = method.CreateDelegate<Func<TModule, ModuleStateContext, Task<StateResult>>>();

    protected override async Task<StateResult> ExecuteInternalAsync(MessageStateContext ctx, CancellationToken cancellationToken = default)
    {
        using var module = CreateModule(ctx);

        var moduleContext = await GetModuleStateContextAsync(ctx, cancellationToken).ConfigureAwait(false);

        if (CheckBack(moduleContext, module, out var back))
            return back;
        if (CheckCancel(moduleContext, module, out var cancel))
            return cancel;

        return await _expression(module, moduleContext).ConfigureAwait(false);
    }
}
