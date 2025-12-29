using System.Globalization;
using BotForge.Fsm;
using BotForge.Modules;
using BotForge.Modules.Attributes;
using BotForge.Modules.Contexts;
using BotForge.Telegram.InformationalBot.Services;
using BotForge.Messaging;

namespace BotForge.Telegram.InformationalBot.Modules;

internal class WeatherModule(IWeatherService weather) : ModuleBase
{
    [MenuItem(nameof(Labels.GetCurrentWeather))]
    [MenuItem(nameof(Labels.GetWeatherForecast))]
    public override StateResult OnModuleRoot(SelectionStateContext ctx) => ctx.Selection() switch
    {
        nameof(Labels.GetCurrentWeather) => ToStateWith(ctx, OnInputLocationAsync, WeatherAction.GetWeather, null),
        nameof(Labels.GetWeatherForecast) => ToStateWith(ctx, OnInputLocationAsync, WeatherAction.GetForecast, null),
        _ => InvalidInput(ctx),
    };

    [Prompt<string>(nameof(Properties.Localization.InputWeatherLocation))]
    public async Task<StateResult> OnInputLocationAsync(PromptStateContext<string> ctx, CancellationToken cancellationToken)
    {
        if (!ctx.TryGetData(out WeatherAction action))
            return Fail(ctx);

        if (!ctx.Input.TryGetValue(out var city))
            return InvalidInput(ctx);

        string result = action switch
        {
            WeatherAction.GetWeather => await GetLiveWeatherStringAsync(ctx.User.TargetLocale, city, cancellationToken).ConfigureAwait(false),
            WeatherAction.GetForecast => await GetForecastStringAsync(ctx.User.TargetLocale, city, cancellationToken).ConfigureAwait(false),
            _ => string.Empty // basically impossible
        };

        return Completed(result);
    }

    private async Task<string> GetLiveWeatherStringAsync(CultureInfo targetCulture, string city, CancellationToken cancellationToken)
        => await weather.GetCurrentWeatherAsync(targetCulture, city, cancellationToken).ConfigureAwait(false);

    private async Task<string> GetForecastStringAsync(CultureInfo targetCulture, string city, CancellationToken cancellationToken)
        => await weather.GetForecastAsync(targetCulture, city, cancellationToken).ConfigureAwait(false);

    internal enum WeatherAction
    {
        GetWeather,
        GetForecast
    }
}
