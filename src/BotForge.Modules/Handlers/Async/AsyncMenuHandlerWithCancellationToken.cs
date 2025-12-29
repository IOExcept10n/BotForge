using System.Reflection;
using BotForge.Fsm;
using BotForge.Messaging;
using BotForge.Modules.Attributes;
using BotForge.Modules.Contexts;

namespace BotForge.Modules.Handlers.Async;

internal class AsyncMenuHandlerWithCancellationToken<TModule>(MethodInfo method, ILabelStore labelStore, IEnumerable<MenuRowAttribute> buttonRows) : ModuleHandlerBase<TModule> where TModule : ModuleBase
{
    private readonly Func<TModule, SelectionStateContext, CancellationToken, Task<StateResult>> _expression = method.CreateDelegate<Func<TModule, SelectionStateContext, CancellationToken, Task<StateResult>>>();
    private readonly List<(string, ButtonLabel)> _buttons = [.. from menuRow in buttonRows from key in menuRow.LabelKeys select (key, labelStore.GetLabel(key))];

    protected override async Task<StateResult> ExecuteInternalAsync(MessageStateContext ctx, CancellationToken cancellationToken = default)
    {
        using var module = CreateModule(ctx);

        var moduleContext = await GetModuleStateContextAsync(ctx, cancellationToken).ConfigureAwait(false);

        if (CheckBack(moduleContext, module, out var back))
            return back;

        var inputContext = moduleContext.ToSelectionContext(_buttons);

        return await _expression(module, inputContext, cancellationToken).ConfigureAwait(false);
    }
}
