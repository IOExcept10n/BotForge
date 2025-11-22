using BotForge.Hosting;
using BotForge.Modules;
using BotForge.Telegram;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var builder = BotApp.CreateBuilder(args).WithTelegramBot();
builder.Configuration.AddUserSecrets<Program>();
// Because our app is single-module, we can simplify menu by removing module selection state
// (Actually this state remains, but it just binds to a single module root state to perform the same logic).
builder.Services.SkipModuleSelection();

var host = builder.Build();
await host.RunAsync().ConfigureAwait(false);

