# BotForge.Hosting
[![NuGet - BotForge.Hosting](https://img.shields.io/nuget/v/BotForge.Hosting.svg?label=BotForge.Hosting)](https://www.nuget.org/packages/BotForge.Hosting/)

Package that adds extensions for setting up a hosted application with BotForge framework. It contains some middlewares for debugging and provides the simplest way to create a new bot app.

Installation
- .NET target: net10.0
- Install: dotnet add package BotForge.Hosting

Quick start
    using BotForge.Hosting;
    using BotForge;

    var builder = BotApp.CreateBuilder(args);

    // Core
    builder.Services
        .AddDefaultStorages()
        .AddDefaultUpdateHandlers()
        .ConfigureUpdatePipeline(p => { /* pipeline config */ });

    // Optional helpers
    builder
        .SkipModuleSelection()         // Use when your app has a single module
        .UseWelcomeMessage("Welcome"); // Simple welcome when roles are not configured

    var app = builder.Build();
    await app.RunAsync();

Highlights
- BotApp.CreateBuilder(...) for convenient host creation
- IBotBuilder extensions for common scenarios (SkipModuleSelection, UseWelcomeMessage)
- Works with Modules, Persistence and integrations (Telegram/Discord)
