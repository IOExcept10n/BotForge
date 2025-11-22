using System.Reflection;
using BotForge.Fsm;
using BotForge.Modules.Contexts;

namespace BotForge.Modules.Handlers.Async;

internal class AsyncCustomHandlerWithCancellationToken<TModule>(MethodInfo method) : ModuleHandlerBase<TModule> where TModule : ModuleBase
{
    private readonly Func<TModule, ModuleStateContext, CancellationToken, Task<StateResult>> _expression = method.CreateDelegate<Func<TModule, ModuleStateContext, CancellationToken, Task<StateResult>>>();

    protected override async Task<StateResult> ExecuteInternalAsync(MessageStateContext ctx, CancellationToken cancellationToken = default)
    {
        using var module = CreateModule(ctx);

        var moduleContext = await GetModuleStateContextAsync(ctx, cancellationToken).ConfigureAwait(false);

        return await _expression(module, moduleContext, cancellationToken).ConfigureAwait(false);
    }
}
