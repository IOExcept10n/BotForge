# BotForge.Telegram
[![NuGet - BotForge.Telegram](https://img.shields.io/nuget/v/BotForge.Telegram.svg?label=BotForge.Telegram)](https://www.nuget.org/packages/BotForge.Telegram/)

An infrastructure package that allows you to simply create Telegram bots using the BotForge framework. It includes the most common BotForge packages set to begin making bot apps.

Installation
- .NET target: net10.0
- Install: dotnet add package BotForge.Telegram

Quick start
    using BotForge.Hosting;
    using BotForge.Telegram;
    using BotForge;

    var builder = BotApp.CreateBuilder(args);

    builder.Services
        .AddDefaultStorages()
        .AddDefaultUpdateHandlers()
        .ConfigureUpdatePipeline(_ => { });

    builder.WithTelegramBot(); // reads ApiKeys:Telegram from configuration
    // or: builder.WithTelegramBot("YOUR_BOT_TOKEN");

    var app = builder.Build();
    await app.RunAsync();
