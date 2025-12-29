using BotForge.Messaging;

namespace BotForge.Telegram.InformationalBot;

// This is a sample button label storage class.
// You can separate these labels to different classes and mark each with [LabelStorage] – all of them will be loaded at startup.
[LabelStorage]
internal class Labels
{
    #region Module buttons

    public static ButtonLabel Weather => new(Emoji.Cloud);
    public static ButtonLabel News => new(Emoji.Newspaper);
    public static ButtonLabel Currency => new(Emoji.CurrencyExchange);

    #endregion


    #region Weather related buttons

    public static ButtonLabel GetCurrentWeather => new(Emoji.Calendar);
    public static ButtonLabel GetWeatherForecast => new(Emoji.SunBehindCloud);

    #endregion


    #region News related buttons

    public static ButtonLabel GetLatestNews => new(Emoji.InformationDeskPerson);

    #endregion


    #region Currency related buttons

    public static ButtonLabel GetExchangeRates => new(Emoji.BanknoteWithDollarSign);
    public static ButtonLabel ConvertPrice => new(Emoji.MoneyBag);

    #endregion


    #region Utilitary buttons

    public static ButtonLabel SetLocale => new(Emoji.GlobeWithMeridians);
    public static ButtonLabel Unknown => new(Emoji.WhiteQuestionMarkOrnament);
    public static ButtonLabel Preferences => new(Emoji.Wrench);
    #endregion

    #region Locales
    public static ButtonLabel LocaleEn => new(Emoji.RegionalIndicatorSymbolLetterURegionalIndicatorSymbolLetterS);
    public static ButtonLabel LocaleRu => new(Emoji.RegionalIndicatorSymbolLetterRRegionalIndicatorSymbolLetterU);
    public static ButtonLabel LocaleSystem => new(Emoji.GlobeWithMeridians);
    #endregion
}
