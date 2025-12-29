# BotForge.Modules

Package that adds a modules system for automatic state generation for the [BotForge framework](https://github.com/IOExcept10n/BotForge). It also adds a role model to authenticate users.

## Installation
- .NET target: net10.0
- Install: `dotnet add package BotForge.Modules`

## Configure modules
```cs
using BotForge.Modules;
using BotForge.Modules.Roles;
using BotForge;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddDefaultStorages()
    .AddDefaultUpdateHandlers()
    .ConfigureUpdatePipeline(_ => { })
    .ConfigureModules<IModuleRegistryBuilder>(b =>
    {
        // Register modules, e.g. scanning assemblies
        // b.UseAssembly(typeof(Program).Assembly);
    })
    .ConfigureRoles<IRoleCatalogBuilder>(roles =>
    {
        roles.AddRole(Role.Unknown, "Welcome"); // default role
        // roles.AddRole("admin", "Admin");
    });

// or use fallbacks:
// builder.Services.AddFallbackModuleConfiguration();
// builder.Services.AddDefaultRolesStorage();

var app = builder.Build();
await app.RunAsync();
```

## Notes
- Use attributes like `[Menu]`, `[Prompt<T>]`, `[ModelPrompt<T>]`, and `[CustomState]` to describe states.
- Default main menu can be simplified if your app has a single module (see Hosting: `SkipModuleSelection`).
- You can simplify configuration by using `BotForge.Hosting` package: it configures all fallback services for you.
