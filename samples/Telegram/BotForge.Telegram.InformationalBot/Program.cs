using BotForge;
using BotForge.Hosting;
using BotForge.Hosting.Middleware;
using BotForge.Modules;
using BotForge.Persistence;
using BotForge.Telegram;
using BotForge.Telegram.InformationalBot.Properties;
using BotForge.Telegram.InformationalBot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// This line is a simple example of how to make app host builder for telegram bot
var builder = BotApp.CreateBuilder(args).WithTelegramBot();

// To run your bot with this sample, just add token to user secrets:
// dotnet user-secrets set "ApiKeys:Telegram" "<Your-Token>"
builder.Configuration.AddUserSecrets<Program>();

// Add errors logging middleware to log all exceptions in message handling process.
builder.Services.ConfigureUpdatePipeline(b => b.UseMiddleware<ErrorLoggingMiddleware>());

// Use resource-based localization for all messages and modules.
builder.Services.AddLocalization(Localization.ResourceManager);

// Add module-related services.
// Don't forget to add API keys to user-secrets for the following services:
// 1. https://weatherapi.com/ - "ApiKeys:WeatherAPI", for weather;
// 2. https://newsapi.org/ - "ApiKeys:NewsApi", for news;
// 3. https://exchangerate.host/ - "ApiKeys:ExchangeRate", for exchange rates.
builder.Services.AddScoped<HttpClient>();
builder.Services.AddScoped<IWeatherService, WeatherService>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<INewsService, NewsService>();

// To persist bot data between runs, just add this call:
builder.AddPersistence();

// These two lines run our bot
var host = builder.Build();
await host.RunAsync().ConfigureAwait(false);

