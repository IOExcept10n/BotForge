using BotForge.Fsm;
using BotForge.Modules;
using BotForge.Modules.Attributes;
using BotForge.Modules.Contexts;

namespace BotForge.Telegram.PingPongBot;

internal class PingPongModule : ModuleBase
{
    [MenuItem("Ping")]
    public override StateResult OnModuleRoot(ModuleStateContext ctx) => RetryWithMessage(ctx, "Pong!");
}
