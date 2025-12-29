namespace BotForge.Telegram.InformationalBot.Services;

/// <summary>
/// Defines a service for retrieving currency exchange rates and checking currency support.
/// </summary>
internal interface ICurrencyService
{
    /// <summary>
    /// Asynchronously retrieves the exchange rate from one currency to another.
    /// </summary>
    /// <param name="fromCurrency">The currency code for the source currency (e.g., "USD").</param>
    /// <param name="toCurrency">The currency code for the target currency (e.g., "EUR").</param>
    /// <param name="cancellationToken">A token that allows the operation to be canceled.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a <see cref="double"/> representing the exchange rate.
    /// </returns>
    Task<double> GetExchangeRatesAsync(string fromCurrency, string toCurrency, CancellationToken cancellationToken);

    /// <summary>
    /// Checks if the service supports a specific currency.
    /// </summary>
    /// <param name="currency">The currency code to check (e.g., "GBP").</param>
    /// <returns><c>true</c> if the service supports the specified currency; otherwise, <c>false</c>.</returns>
    bool SupportsCurrency(string currency);
}
