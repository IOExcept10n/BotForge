using BotForge.Fsm;
using BotForge.Modules;
using BotForge.Modules.Attributes;
using BotForge.Modules.Contexts;
using BotForge.Messaging;
using BotForge.Telegram.InformationalBot.Services;

namespace BotForge.Telegram.InformationalBot.Modules;

internal sealed class NewsModule(INewsService newsService) : ModuleBase
{
    private readonly INewsService _newsService = newsService;

    [MenuItem(nameof(Labels.GetLatestNews))]
    public override async Task<StateResult> OnModuleRootAsync(SelectionStateContext ctx, CancellationToken ct)
    {
        string news = await _newsService.GetNewsStringAsync(ctx.User.TargetLocale, ct).ConfigureAwait(false);
        return RetryWithMessage(ctx, news);
    }
}
