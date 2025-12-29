# BotForge.Core

Core package of the [BotForge framework](https://github.com/IOExcept10n/BotForge). This package includes basic classes for FSM architecture, messaging wrappers for platform-specific input, features for localization and pipeline for multithreaded asynchronous update handling. This package is platform-independent and used for any messaging system like Telegram or Discord.

## Installation
- .NET target: net10.0
- Install: `dotnet add package BotForge.Core`

## What it provides
- FSM and state registry
- Update processing pipeline (configurable)
- Messaging abstractions (`IMessage`, `IInteraction`, `IReplyChannel`, `IUpdateChannel`, `ITransport`)
- In-memory defaults for user state and locale
- Simple localization service with ResourceManager binding

If you want to add a ready-to-use version of the package, please select your target platform and consider using platform-related package, like `BotForge.Telegram`.
