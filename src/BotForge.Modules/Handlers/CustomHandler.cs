using System.Reflection;
using BotForge.Fsm;
using BotForge.Modules.Contexts;

namespace BotForge.Modules.Handlers;

internal class CustomHandler<TModule>(MethodInfo method) : ModuleHandlerBase<TModule> where TModule : ModuleBase
{
    private readonly Func<TModule, ModuleStateContext, StateResult> _expression = method.CreateDelegate<Func<TModule, ModuleStateContext, StateResult>>();

    protected override async Task<StateResult> ExecuteInternalAsync(MessageStateContext ctx, CancellationToken cancellationToken = default)
    {
        using var module = CreateModule(ctx);

        var moduleContext = await GetModuleStateContextAsync(ctx, cancellationToken).ConfigureAwait(false);

        return _expression(module, moduleContext);
    }
}
