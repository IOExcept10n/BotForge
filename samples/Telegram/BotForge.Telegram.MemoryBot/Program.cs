using BotForge.Hosting;
using BotForge.Modules;
using BotForge.Modules.Roles;
using BotForge.Persistence;
using BotForge.Telegram;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

// This line is a simple example of how to make app host for telegram bot
var builder = BotApp.CreateBuilder(args).WithTelegramBot();

// To run your bot with this sample, just add token to user secrets:
// dotnet user-secrets set "ApiKeys:Telegram" "<Your-Token>"
builder.Configuration.AddUserSecrets<Program>();

// Because our app is single-module, we can simplify menu by removing module selection state
// (Actually this state remains, but it just binds to a single module root state to perform the same logic).
builder.SkipModuleSelection();

// We can configure welcome message for all users if not configuring roles.
builder.UseWelcomeMessage(
        "👋 Welcome to the **Persistence Demo Bot**!\n\n" +
        "This bot demonstrates how BotForge persistence works.\n\n" +
        "✨ Your state is automatically saved to a database\n" +
        "🔄 Values persist across bot restarts\n" +
        "💾 No manual database code needed!\n\n" +
        "📋 Choose an action:");

// To persist bot data between runs, just add this call:
builder.AddPersistence();

// These two lines run our bot
var host = builder.Build();
await host.RunAsync().ConfigureAwait(false);

