using System.ComponentModel.DataAnnotations;
using BotForge.Fsm;
using BotForge.Modules;
using BotForge.Modules.Attributes;
using BotForge.Modules.Contexts;
using BotForge.Telegram.InformationalBot.Services;

namespace BotForge.Telegram.InformationalBot.Modules;

internal sealed class CurrencyModule(ICurrencyService currency) : ModuleBase
{
    private readonly ICurrencyService _currency = currency;

    [MenuItem(nameof(Labels.GetExchangeRates))]
    [MenuItem(nameof(Labels.ConvertPrice))]
    public override async Task<StateResult> OnModuleRootAsync(SelectionStateContext ctx, CancellationToken cancellationToken)
    {
        if (ctx.Matches(Labels.GetExchangeRates))
        {
            string rates = $"""
{Properties.Localization.ActualExchangeRates}:
{await GetExchangeRateLine("USD", "EUR", cancellationToken).ConfigureAwait(false)}
{await GetExchangeRateLine("USD", "GBP", cancellationToken).ConfigureAwait(false)}
{await GetExchangeRateLine("USD", "JPY", cancellationToken).ConfigureAwait(false)}
""";
            return RetryWithMessage(ctx, rates);
        }

        return ToState(ctx, OnConvertCurrencyAsync);
    }

    [ModelPrompt<CurrencyConversionModel>]
    public async Task<StateResult> OnConvertCurrencyAsync(ModelPromptContext<CurrencyConversionModel> ctx, CancellationToken cancellationToken)
    {
        var rate = await _currency.GetExchangeRatesAsync(ctx.Model.FromCode, ctx.Model.ToCode, cancellationToken).ConfigureAwait(false);
        string result = $"{ctx.Model.Amount} {ctx.Model.FromCode} = {ctx.Model.Amount * rate} {ctx.Model.ToCode}";
        return Completed(result);
    }

    protected override Task<bool> ValidateModelAsync(ModelBindContext ctx, ModelBindingBuilder bindingBuilder, ICollection<ValidationResult> errors)
    {
        if (bindingBuilder.InputProperty.Property.Name is nameof(CurrencyConversionModel.FromCode) or nameof(CurrencyConversionModel.ToCode) &&
            !_currency.SupportsCurrency(bindingBuilder.GetValidatedProperty<string>()))
        {
            errors.Add(new(Properties.Localization.CurrencyNotSupported));
            return Task.FromResult(false);
        }
        return Task.FromResult(true);
    }

    private async Task<string> GetExchangeRateLine(string from, string to, CancellationToken ct) => $"{from} - {to}: {await _currency.GetExchangeRatesAsync(from, to, ct).ConfigureAwait(false)}";

    internal readonly record struct CurrencyConversionModel(
        [property: Display(Prompt = nameof(Properties.Localization.InputFromCode))] string FromCode,
        [property: Display(Prompt = nameof(Properties.Localization.InputToCode))] string ToCode,
        [property: Display(Prompt = nameof(Properties.Localization.InputAmount)), Range(0, double.PositiveInfinity)] double Amount);
}
