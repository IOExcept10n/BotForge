# BotForge.Core
[![NuGet - BotForge.Core](https://img.shields.io/nuget/v/BotForge.Core.svg?label=BotForge.Core)](https://www.nuget.org/packages/BotForge.Core/)

Core package of the BotForge framework. This package includes basic classes for FSM architecture, messaging wrappers for platform-specific input, features for localization and pipeline for multithreaded asynchronous update handling. This package is platform-independent and used for any messaging system like Telegram or Discord.

Installation
- .NET target: net10.0
- Install: dotnet add package BotForge.Core

Quick start
    using BotForge;
    using Microsoft.Extensions.Hosting;

    var builder = Host.CreateApplicationBuilder(args);

    // Core services
    builder.Services
        .AddDefaultStorages()
        .AddDefaultUpdateHandlers()
        .ConfigureUpdatePipeline(p =>
        {
            // Configure FSM/middleware here if needed
        });

    var app = builder.Build();
    await app.RunAsync();

What it provides
- FSM and state registry
- Update processing pipeline (configurable)
- Messaging abstractions (IMessage, IInteraction, IReplyChannel, IUpdateChannel, ITransport)
- In-memory defaults for user state and locale
- Simple localization service with ResourceManager binding
