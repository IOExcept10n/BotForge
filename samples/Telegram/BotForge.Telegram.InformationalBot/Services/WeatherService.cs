using System.Globalization;
using System.Net.Http.Json;
using BotForge.Localization;
using BotForge.Messaging;
using Microsoft.Extensions.Configuration;

namespace BotForge.Telegram.InformationalBot.Services;

internal class WeatherService(HttpClient httpClient, IConfiguration configuration, ILocalizationService localization) : IWeatherService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILocalizationService _localization = localization;
    private readonly string _apiKey = configuration["ApiKeys:WeatherAPI"] ?? throw new InvalidOperationException("WeatherAPI key is missing");

    public async Task<string> GetCurrentWeatherAsync(CultureInfo targetCulture, string city, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<WeatherResponse>(
                $"https://api.weatherapi.com/v1/current.json?key={_apiKey}&q={city}&aqi=no",
                cancellationToken).ConfigureAwait(false);

            if (response == null) return _localization.GetString(targetCulture, nameof(Properties.Localization.WeatherDataNotAvailable));

            return $"{Emoji.SunBehindCloud.ToUnicode()} {_localization.GetString(targetCulture, nameof(Properties.Localization.CurrentWeather))} {city}:\n" +
                   $"- {_localization.GetString(targetCulture, nameof(Properties.Localization.Condition))}: {response.Current.Condition.Text}\n" +
                   $"- {_localization.GetString(targetCulture, nameof(Properties.Localization.Temperature))}: {response.Current.Temp_C} °C\n" +
                   $"- {_localization.GetString(targetCulture, nameof(Properties.Localization.Humidity))}: {response.Current.Humidity}%";
        }
        catch (Exception ex) when (ex is not TaskCanceledException)
        {
            return $"{Properties.Localization.ErrorFetchingWeather}: {ex.Message}";
        }
    }

    public async Task<string> GetForecastAsync(CultureInfo targetCulture, string city, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<ForecastResponse>(
                $"https://api.weatherapi.com/v1/forecast.json?key={_apiKey}&q={city}&days=5&aqi=no&alerts=no",
                cancellationToken).ConfigureAwait(false);

            if (response == null) return _localization.GetString(targetCulture, nameof(Properties.Localization.WeatherDataNotAvailable));

            string forecast = $"{Emoji.Calendar.ToUnicode()} {_localization.GetString(targetCulture, nameof(Properties.Localization.Forecast))} {city}:\n";
            foreach (var day in response.Forecast.Forecastday)
            {
                forecast += $"• {day.Date}:\n" +
                            $"- {_localization.GetString(targetCulture, nameof(Properties.Localization.Condition))}: {day.Day.Condition.Text}\n" +
                            $"- {_localization.GetString(targetCulture, nameof(Properties.Localization.MaxTemperature))}: {day.Day.MaxTemp_C} °C\n" +
                            $"- {_localization.GetString(targetCulture, nameof(Properties.Localization.MinTemperature))}: {day.Day.MinTemp_C} °C\n";
            }
            return forecast;
        }
        catch (Exception ex) when (ex is not TaskCanceledException)
        {
            return $"{_localization.GetString(targetCulture, nameof(Properties.Localization.ErrorFetchingForecast))}: {ex.Message}";
        }
    }

    private class WeatherResponse
    {
        public required CurrentInfo Current { get; set; }
    }

    private class CurrentInfo
    {
        public required ConditionInfo Condition { get; set; }
        public float Temp_C { get; set; }
        public int Humidity { get; set; }
    }

    private class ConditionInfo
    {
        public required string Text { get; set; }
    }

    private class ForecastResponse
    {
        public required ForecastInfo Forecast { get; set; }
    }

    private class ForecastInfo
    {
        public required ForecastDayInfo[] Forecastday { get; set; }
    }

    private class ForecastDayInfo
    {
        public required string Date { get; set; }
        public required DayInfo Day { get; set; }
    }

    private class DayInfo
    {
        public required ConditionInfo Condition { get; set; }
        public float MaxTemp_C { get; set; }
        public float MinTemp_C { get; set; }
    }
}
