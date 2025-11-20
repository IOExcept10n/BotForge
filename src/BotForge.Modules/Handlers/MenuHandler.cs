using System.Reflection;
using BotForge.Fsm;
using BotForge.Messaging;
using BotForge.Modules.Attributes;
using BotForge.Modules.Contexts;

namespace BotForge.Modules.Handlers;

internal class MenuHandler<TModule>(MethodInfo method, ILabelStore labelStore) : ModuleHandlerBase<TModule> where TModule : ModuleBase
{
    private readonly Func<TModule, SelectionStateContext, StateResult> _expression = method.CreateDelegate<Func<TModule, SelectionStateContext, StateResult>>();
    private readonly List<(string, ButtonLabel)> _buttons = [.. from menuRow in method.GetCustomAttributes<MenuRowAttribute>() from key in menuRow.LabelKeys select (key, labelStore.GetLabel(key))];

    public override async Task<StateResult> ExecuteAsync(MessageStateContext ctx, CancellationToken cancellationToken = default)
    {
        using var module = CreateModule(ctx);

        var moduleContext = await GetModuleStateContextAsync(ctx, cancellationToken).ConfigureAwait(false);
        if (CheckBack(moduleContext, module, out var back))
            return back;
        var inputContext = moduleContext.ToSelectionContext(_buttons);

        return _expression(module, inputContext);
    }
}
