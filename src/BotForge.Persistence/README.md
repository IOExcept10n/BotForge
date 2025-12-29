# BotForge.Persistence

Package that adds database persistence for bots created with the [BotForge framework](https://github.com/IOExcept10n/BotForge). It contains a basic DbContext for storing user states and identification info.

## Installation
- .NET target: net10.0
- Install: `dotnet add package BotForge.Persistence`

## Quick start (SQLite by default)
```
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
```

## Customization (options)
```cs
builder.AddPersistence(options => options
    .UseConnectionString("Data Source=mybot.db") // Pass custom connection string
    .AutoMigrate(true) // Automatically migrate on startup
    .UseEnsureCreated(false) // Use EnsureCreated on database
    .ReplaceUserStateStore() // Replace user state storage with persistent one (true by default)
    .ReplaceUserLocaleProvider() // Replace user locale provider with persistent one (true by default)
    .ReplaceRoleStorage()); // Replace roles storage with persistent one (true by default)
```

## Customization (custom DbContext)
```cs
builder.AddPersistence<MyDbContext>(opt => opt
    .UseMigrationsAssembly(typeof(MyDbContext).Assembly) // Use custom assembly for migrations (inferred by DbContext type by default)
    .ScanRepositoriesFrom(typeof(MyDbContext).Assembly)); // Scan repository types from specific assembly (inferred by DbContext type by default)
```

## Configuration
- Reads connection string from:
  - `ConnectionStrings:BotForge`
  - `ConnectionStrings:DefaultConnection`
