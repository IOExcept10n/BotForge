using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;

namespace BotForge.Telegram.InformationalBot.Services;

// Uses exchangerate.host
// Docs: https://exchangerate.host/#/
internal sealed class CurrencyService(HttpClient httpClient, IConfiguration config) : ICurrencyService
{
    private readonly HttpClient _http = httpClient;
    private readonly string? _apiKey = config["ApiKeys:ExchangeRate"];
    private static readonly ConcurrentDictionary<string, double> CachedRates = new();

    private static readonly HashSet<string> Supported = new(StringComparer.OrdinalIgnoreCase)
    {
        "USD","EUR","GBP","JPY","UAH","PLN","CHF","CAD","AUD","CNY","SEK","NOK","DKK","CZK","HUF","ILS","TRY","RON","BGN","HRK","MXN","BRL","INR","ZAR","KRW","HKD","SGD","NZD"
    };

    /// <inheritdoc/>
    public bool SupportsCurrency(string code) => !string.IsNullOrWhiteSpace(code) && Supported.Contains(code);

    /// <inheritdoc/>
    public async Task<double> GetExchangeRatesAsync(string from, string to, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
            throw new ArgumentException("Currency codes must be provided");

        string key = $"{from}{to}";

        if (CachedRates.TryGetValue(key, out double cachedRate))
        {
            return cachedRate;
        }

        var url = $"https://api.exchangerate.host/live?access_key={_apiKey}";
        var resp = await _http.GetFromJsonAsync<LiveResponse>(url, cancellationToken).ConfigureAwait(false);

        if (resp is null || !resp.Success || resp.Quotes is null)
            throw new InvalidOperationException("Exchange rate data is not available");

        foreach (var quote in resp.Quotes)
        {
            CachedRates.TryAdd(quote.Key, quote.Value);
        }

        string exchangeKey = $"{from}{to}";
        if (CachedRates.TryGetValue(exchangeKey, out double result))
        {
            return result;
        }

        throw new InvalidOperationException("Exchange rate not found");
    }

    private sealed class LiveResponse
    {
        public bool Success { get; set; }
        public Dictionary<string, double>? Quotes { get; set; }
    }
}
