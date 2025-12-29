using System.Globalization;
using BotForge.Fsm;
using BotForge.Localization;
using BotForge.Modules;
using BotForge.Modules.Attributes;
using BotForge.Modules.Contexts;

namespace BotForge.Telegram.InformationalBot.Modules;

[Module(nameof(Labels.Preferences), Order = 1)]
internal class PreferencesModule(IUserLocaleProvider localeProvider) : ModuleBase
{
    private readonly IUserLocaleProvider _localeProvider = localeProvider;

    [MenuItem(nameof(Labels.SetLocale))]
    public override StateResult OnModuleRoot(SelectionStateContext ctx) => ctx.Selection() switch
    {
        nameof(Labels.SetLocale) => ToState(ctx, OnSetLocale),
        _ => InvalidInput(ctx)
    };

    [Menu(nameof(Properties.Localization.SelectNewLocale))]
    [MenuItem(nameof(Labels.LocaleEn))]
    [MenuItem(nameof(Labels.LocaleRu))]
    [MenuItem(nameof(Labels.LocaleSystem))]
    public async Task<StateResult> OnSetLocale(SelectionStateContext ctx, CancellationToken cancellationToken)
    {
        CultureInfo? preferredLocale = ctx.Selection() switch
        {
            nameof(Labels.LocaleEn) => new("en-US"),
            nameof(Labels.LocaleRu) => new("ru-RU"),
            nameof(Labels.LocaleSystem) => null,
            _ => await _localeProvider.GetPreferredLocaleAsync(ctx.User, cancellationToken).ConfigureAwait(false),
        };

        await _localeProvider.SetPreferredLocaleAsync(ctx.User, preferredLocale, cancellationToken).ConfigureAwait(false);
        return Completed(nameof(Properties.Localization.LocaleSet));
    }
}
