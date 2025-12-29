using System.Globalization;

namespace BotForge.Telegram.InformationalBot.Services;

/// <summary>
/// Defines a service for retrieving current weather and forecasts for specific locations.
/// </summary>
internal interface IWeatherService
{
    /// <summary>
    /// Asynchronously retrieves the current weather for a specified city in the given culture.
    /// </summary>
    /// <param name="targetCulture">The <see cref="CultureInfo"/> representing the desired culture for weather information (e.g., for formatting).</param>
    /// <param name="city">The name of the city for which to retrieve the current weather.</param>
    /// <param name="cancellationToken">A token that allows the operation to be canceled.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a string representing the current weather information formatted according to the specified culture.
    /// </returns>
    Task<string> GetCurrentWeatherAsync(CultureInfo targetCulture, string city, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously retrieves the weather forecast for a specified city in the given culture.
    /// </summary>
    /// <param name="targetCulture">The <see cref="CultureInfo"/> representing the desired culture for weather information (e.g., for formatting).</param>
    /// <param name="city">The name of the city for which to retrieve the weather forecast.</param>
    /// <param name="cancellationToken">A token that allows the operation to be canceled.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a string representing the weather forecast information formatted according to the specified culture.
    /// </returns>
    Task<string> GetForecastAsync(CultureInfo targetCulture, string city, CancellationToken cancellationToken);
}
