# BotForge.Modules
[![NuGet - BotForge.Modules](https://img.shields.io/nuget/v/BotForge.Modules.svg?label=BotForge.Modules)](https://www.nuget.org/packages/BotForge.Modules/)

Package that adds a modules system for automatic state generation for the BotForge framework. It also adds a role model to authenticate users.

Installation
- .NET target: net10.0
- Install: dotnet add package BotForge.Modules

Configure modules
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

Notes
- Use attributes like [Menu], [Prompt<T>], [ModelPrompt<T>], and [CustomState] to describe states.
- Default main menu can be simplified if your app has a single module (see Hosting: SkipModuleSelection).
