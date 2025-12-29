using System.Threading.Tasks;
using BotForge.Fsm;
using BotForge.Modules;
using BotForge.Modules.Attributes;
using BotForge.Modules.Contexts;

namespace BotForge.Telegram.PingPongBot;

// This is your module — main logical block of the bot. It handles user commands and performs routing.
internal sealed class PingPongModule : ModuleBase
{
    // Module root is an entry point of each module. You can use this function to direct user using menu buttons.
    [MenuItem("Ping")]
    public override StateResult OnModuleRoot(SelectionStateContext ctx) => RetryWithMessage(ctx, "Pong!");
}
