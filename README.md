# 🤖 BotForge

![BotForge icon](res/icon_flat.png)

[![NuGet - BotForge.Core](https://img.shields.io/nuget/v/BotForge.Core.svg?label=BotForge.Core)](https://www.nuget.org/packages/BotForge.Core/)

BotForge is a modular .NET chatbot framework powered by a finite state machine (FSM) architecture.
It started as an internal tool for a university hackathon and evolved into an open-source project for developers who want structured, modular, and declarative bot logic.

Status
- Active development
- Pre-release packages published on NuGet

NuGet packages
- BotForge.Analyzers: https://www.nuget.org/packages/BotForge.Analyzers/
- BotForge.Core: https://www.nuget.org/packages/BotForge.Core/
- BotForge.Hosting: https://www.nuget.org/packages/BotForge.Hosting/
- BotForge.Modules: https://www.nuget.org/packages/BotForge.Modules/
- BotForge.Persistence: https://www.nuget.org/packages/BotForge.Persistence/
- BotForge.Telegram.Integration: https://www.nuget.org/packages/BotForge.Telegram.Integration/
- BotForge.Telegram: https://www.nuget.org/packages/BotForge.Telegram/

Getting started (Telegram, minimal)
1) Install
   `dotnet add package BotForge.Telegram`

2) Configure
   Add configuration key `ApiKeys:Telegram` with your bot token (e.g., in appsettings.json or environment variables).

3) Program.cs
```cs
using BotForge;
using BotForge.Fsm;
using BotForge.Hosting;
using BotForge.Modules;
using BotForge.Modules.Attributes;
using BotForge.Modules.Contexts;
using BotForge.Telegram;

var builder = BotApp.CreateBuilder(args).WithTelegramBot(); // reads ApiKeys:Telegram from configuration
// or: builder.WithTelegramBot("YOUR_BOT_TOKEN");

var app = builder.Build();
await app.RunAsync();

class PingPongModule : ModuleBase
{
    [MenuItem("Ping")]
    public override StateResult OnModuleRoot(SelectionStateContext ctx) => RetryWithMessage(ctx, "Pong!");
}
```

Project goals
- Provide a clean FSM architecture for complex conversational flows
- Offer a modular system with declarative state definitions and role-based access
- Deliver Telegram integration with room for other platforms in the future
- Simplify bot development through hosting support, dependency injection, and extensibility

Solution structure
- BotForge.Core: Core FSM engine and dispatching logic
- BotForge.Hosting: Integration with IHostApplicationBuilder and DI container
- BotForge.Modules: Modular system and declarative FSM attributes
- BotForge.Persistence: EF Core-based FSM state storage
- BotForge.Telegram.Integration: Low-level transport mapping for Telegram.Bot
- BotForge.Telegram: Ready-to-use Telegram setup on top of Hosting + Modules + Integration
- BotForge.Analyzers: Roslyn analyzers (and code fixes via meta-package) for FSM attribute validation

Roadmap
See docs/roadmap.md for detailed progress and upcoming milestones.

Build
```sh
git clone https://github.com/IOExcept10n/BotForge.git
cd BotForge
dotnet restore
dotnet build
dotnet test
```

Samples
- Telegram Ping-Pong bot: samples/Telegram/BotForge.Telegram.PingPongBot
- Telegram Informational bot: samples/Telegram/BotForge.Telegram.InformationalBot
- Telegram Persistence demo: samples/Telegram/BotForge.Telegram.MemoryBot

License
This project is licensed under the **MIT License**. See the [LICENSE](LICENSE) file for details.

Author
Developed by @IOExcept10n (https://github.com/IOExcept10n).
Originated as a Telegram bot for a university hackathon project.
