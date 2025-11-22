using System.Diagnostics.CodeAnalysis;
using BotForge.Fsm;
using BotForge.Messaging;
using BotForge.Modules.Contexts;
using BotForge.Modules.Roles;
using Microsoft.Extensions.DependencyInjection;

namespace BotForge.Modules.Handlers;

internal abstract class ModuleHandlerBase<TModule> : IStateHandler where TModule : ModuleBase
{
    public string ModuleName { get; set; }

    public async Task<StateResult> ExecuteAsync(MessageStateContext ctx, CancellationToken cancellationToken = default)
    {
        var result = await ExecuteInternalAsync(ctx, cancellationToken).ConfigureAwait(false);
        if (result.NextStateId == StateRecord.StartStateId)
        {
            result = result with { NextStateId = $"{(await ctx.Services.GetRequiredService<IRoleProvider>().GetRoleAsync(ctx.Message.From)).Name}:{StateRecord.StartStateId}" };
        }
        return result;
    }

    protected abstract Task<StateResult> ExecuteInternalAsync(MessageStateContext ctx, CancellationToken cancellationToken = default);

    protected async Task<ModuleStateContext> GetModuleStateContextAsync(MessageStateContext ctx, CancellationToken cancellationToken = default)
    {
        var user = ctx.Message.From;
        var role = await ctx.Services.GetRequiredService<IRoleProvider>().GetRoleAsync(user, cancellationToken).ConfigureAwait(false);
        return new(user, ctx.Message.ChatId, role, ctx.Message, ctx.CurrentState, ctx.Services);
    }

    protected TModule CreateModule(MessageStateContext context)
    {
        var module = ActivatorUtilities.CreateInstance<TModule>(context.Services);
        module.Name = ModuleName;
        return module;
    }

    protected static bool CheckBack(ModuleStateContext context, ModuleBase module, [NotNullWhen(true)] out StateResult? back)
    {
        var labels = context.Services.GetRequiredService<ILabelStore>();
        if (context.Matches(labels.CancelButton))
        {
            back = module.Back(context);
            return true;
        }
        back = null;
        return false;
    }

    protected static bool CheckCancel(ModuleStateContext context, ModuleBase module, [NotNullWhen(true)] out StateResult? back)
    {
        var labels = context.Services.GetRequiredService<ILabelStore>();
        if (context.Matches(labels.BackButton))
        {
            back = module.Back(context);
            return true;
        }
        back = null;
        return false;
    }
}
