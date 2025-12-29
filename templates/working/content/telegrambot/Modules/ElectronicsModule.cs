using System.ComponentModel.DataAnnotations;
using BotForge.Fsm;
using BotForge.Messaging;
using BotForge.Modules;
using BotForge.Modules.Attributes;
using BotForge.Modules.Contexts;

namespace telegrambot.Modules;

internal sealed class ElectronicsModule(ItemsRegistry items) : ModuleBase
{
    [MenuItem(nameof(Labels.Smartphones))]
    [MenuItem(nameof(Labels.Headphones))]
    [MenuItem(nameof(Labels.Smartwatches))]
    public override StateResult OnModuleRoot(SelectionStateContext ctx) => ctx.Selection() switch
    {
        nameof(Labels.Smartphones) => ToState(ctx),
        nameof(Labels.Headphones) => ToState(ctx),
        nameof(Labels.Smartwatches) => ToState(ctx),
    }
}
