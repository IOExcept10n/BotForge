# BotForge.Telegram

An infrastructure package that allows you to simply create Telegram bots using the [BotForge framework](https://github.com/IOExcept10n/BotForge). It includes the most common BotForge packages set to begin making bot apps.

## Installation
- .NET target: net10.0
- Install: `dotnet add package BotForge.Telegram`

## Quick start
```cs
using BotForge.Hosting;
using BotForge.Telegram;
using BotForge;

var builder = BotApp.CreateBuilder(args).WithTelegramBot(); // reads ApiKeys:Telegram from configuration
// or: builder.WithTelegramBot("YOUR_BOT_TOKEN");

var app = builder.Build();
await app.RunAsync();
```
