# BotForge.Analyzers (Meta Package)

NuGet meta-package that ships BotForge analyzers and code fixes as a development-time dependency.

## Installation
- `dotnet add package BotForge.Analyzers`

## Notes
- Marked as DevelopmentDependency; analyzers run during build/IDE, no runtime impact.
- No changes required in your code; diagnostics and fixes appear automatically.

See src/BotForge.Analyzers/README.md for rules and details.
