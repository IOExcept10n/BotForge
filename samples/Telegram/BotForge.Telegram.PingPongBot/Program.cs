using BotForge.Hosting;
using BotForge.Telegram;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var builder = BotApp.CreateBuilder(args).WithTelegramBot();
builder.Configuration.AddUserSecrets<Program>();

var host = builder.Build();
await host.RunAsync();

