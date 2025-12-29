# BotForge.Persistence
[![NuGet - BotForge.Persistence](https://img.shields.io/nuget/v/BotForge.Persistence.svg?label=BotForge.Persistence)](https://www.nuget.org/packages/BotForge.Persistence/)

Package that adds database persistence for bots created with the BotForge framework. It contains a basic DbContext for storing user states and identification info.

Installation
- .NET target: net10.0
- Install: dotnet add package BotForge.Persistence

Quick start (SQLite by default)
    using BotForge.Persistence;
    using BotForge.Hosting;
    using BotForge;

    var builder = BotApp.CreateBuilder(args);

    builder.Services
        .AddDefaultStorages()
        .AddDefaultUpdateHandlers()
        .ConfigureUpdatePipeline(_ => { });

    builder.AddPersistence(); // SQLite "Data Source=botforge.db" by default

    var app = builder.Build();
    await app.RunAsync();

Customization (options)
    builder.AddPersistence(options => options
        .UseConnectionString("Data Source=mybot.db")
        .AutoMigrate(true)
        .UseEnsureCreated(false)
        .ReplaceUserStateStore()
        .ReplaceUserLocaleProvider()
        .ReplaceRoleStorage());

Customization (custom DbContext)
    builder.AddPersistence<MyDbContext>(opt => opt
        .UseMigrationsAssembly(typeof(MyDbContext).Assembly)
        .ScanRepositoriesFrom(typeof(MyDbContext).Assembly));

Configuration
- Reads connection string from:
  - ConnectionStrings:BotForge
  - ConnectionStrings:DefaultConnection
