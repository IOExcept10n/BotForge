// See https://aka.ms/new-console-template for more information
using BotForge;
using BotForge.Hosting;
using BotForge.Hosting.Middleware;
using BotForge.Modules;
using BotForge.Telegram;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using telegrambot;
using telegrambot.Properties;

// This line is a simple example of how to make app host builder for telegram bot
var builder = BotApp.CreateBuilder(args).WithTelegramBot();

// To run your bot with this sample, just add token to user secrets:
// dotnet user-secrets set "ApiKeys:Telegram" "<Your-Token>"
builder.Configuration.AddUserSecrets<Program>();

// Add errors logging middleware to log all exceptions in message handling process.
builder.Services.ConfigureUpdatePipeline(b => b.UseMiddleware<ErrorLoggingMiddleware>());

// Use resource-based localization for all messages and modules.
builder.Services.AddLocalization(Resources.ResourceManager);

// You can also add any custom services for your app.
builder.Services.AddSingleton<ItemsRegistry>();

// To persist bot data between runs, just include BotForge.Persistence reference and add this call:
// builder.AddPersistence();

// These two lines run our bot
var host = builder.Build();
await host.RunAsync().ConfigureAwait(false);

