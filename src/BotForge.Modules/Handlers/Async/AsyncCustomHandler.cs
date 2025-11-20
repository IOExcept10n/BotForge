using System.Reflection;
using BotForge.Fsm;
using BotForge.Modules.Contexts;

namespace BotForge.Modules.Handlers.Async;

internal class AsyncCustomHandler<TModule>(MethodInfo method) : ModuleHandlerBase<TModule> where TModule : ModuleBase
{
    private readonly Func<TModule, ModuleStateContext, Task<StateResult>> _expression = method.CreateDelegate<Func<TModule, ModuleStateContext, Task<StateResult>>>();

    public override async Task<StateResult> ExecuteAsync(MessageStateContext ctx, CancellationToken cancellationToken = default)
    {
        using var module = CreateModule(ctx);

        var moduleContext = await GetModuleStateContextAsync(ctx, cancellationToken).ConfigureAwait(false);

        return await _expression(module, moduleContext).ConfigureAwait(false);
    }
}
