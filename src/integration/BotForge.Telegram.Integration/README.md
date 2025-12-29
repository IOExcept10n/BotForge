# BotForge.Telegram.Integration
[![NuGet - BotForge.Telegram.Integration](https://img.shields.io/nuget/v/BotForge.Telegram.Integration.svg?label=BotForge.Telegram.Integration)](https://www.nuget.org/packages/BotForge.Telegram.Integration/)

An integration package with Telegram.Bot wrappers for the BotForge framework. It provides a way to map Telegram updates to BotForge updates that are ready for pipeline handling. You may need it to create your custom Telegram handling logic, but for general bots you may need BotForge.Telegram package which includes modular infrastructure and hosting features.

Installation
- .NET target: net10.0
- Install: dotnet add package BotForge.Telegram.Integration

Usage
Register Telegram transport services and ITelegramBotClient:

    using BotForge.Telegram.Integration;
    using Telegram.Bot;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Configuration;
    using BotForge;

    var builder = Host.CreateApplicationBuilder(args);

    // ITelegramBotClient
    builder.Services.AddSingleton<ITelegramBotClient>(sp =>
    {
        var cfg = sp.GetRequiredService<IConfiguration>();
        var token = cfg["ApiKeys:Telegram"];
        ArgumentNullException.ThrowIfNull(token);
        return new TelegramBotClient(token);
    });

    // Transport
    builder.Services
        .AddDefaultStorages()
        .AddDefaultUpdateHandlers()
        .ConfigureUpdatePipeline(_ => { })
        .AddTelegramTransport();

    var app = builder.Build();
    await app.RunAsync();

Prefer BotForge.Telegram if you just need a ready-to-use setup method.
