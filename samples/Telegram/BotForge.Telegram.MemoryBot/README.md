# BotForge Persistence Demo Bot

This sample demonstrates BotForge's persistence functionality with a simple counter bot.

## Features Demonstrated

- ✅ **Automatic State Persistence**: User state is automatically saved to a SQLite database
- ✅ **Persistence Across Restarts**: Counter values persist even after bot restarts
- ✅ **Zero-Configuration**: Just call `AddPersistence()` - no manual database setup needed
- ✅ **Default Migrations**: Database schema is created automatically using embedded migrations

## How It Works

1. **Persistence Setup**: The `Program.cs` file calls `builder.Services.AddPersistence()` which:
   - Configures SQLite database (default: `botforge.db`)
   - Registers persistent implementations of `IUserStateStore`, `IUserLocaleProvider`, and role storage
   - Applies default migrations automatically on startup

2. **State Management**: The `MemoryModule` tracks a counter value in `StateData`:
   - Counter value is stored in `ctx.CurrentState.StateData`
   - BotForge automatically persists this data when state changes
   - On restart, the state is loaded from the database

3. **User Registration**: When a user first interacts with the bot:
   - User is automatically registered in the database
   - User information (ID, username, display name, locale) is merged intelligently
   - `LastSeen` timestamp is updated on every interaction

## Testing Persistence

1. **Start the bot** and interact with it
2. **Increment the counter** a few times
3. **Stop the bot** (Ctrl+C)
4. **Restart the bot**
5. **Check the counter** - it should still show your previous value!

## Database Location

By default, the database is created at: `botforge.db` (in the application directory)

You can customize this in `Program.cs`:

```csharp
builder.Services.AddPersistence(options => options
    .UseConnectionString("Data Source=mybot.db"));
```

Or read from configuration:

```csharp
// In appsettings.json:
// {
//   "ConnectionStrings": {
//     "BotForge": "Data Source=mybot.db"
//   }
// }
```

## What Gets Persisted

- **User State**: Current state ID and state data (JSON)
- **User Information**: Platform ID, username, display name, discriminator
- **User Preferences**: Preferred locale, original locale
- **User Roles**: Role assignments
- **Timestamps**: Created date, last seen date

## Customization

You can customize persistence behavior:

```csharp
builder.Services.AddPersistence(options => options
    .UseConnectionString("Data Source=custom.db")
    .ReplaceUserStateStore()      // Replace IUserStateStore (default: true)
    .ReplaceUserLocaleProvider()   // Replace IUserLocaleProvider (default: true)
    .ReplaceRoleStorage()          // Replace role storage (default: true)
    .AutoMigrate(true)             // Auto-apply migrations (default: true)
    .UseEnsureCreated(false));     // Use EnsureCreated instead (default: false)
```

## Next Steps

- Try adding custom entities and repositories
- Create custom migrations for your own entities
- Use a different database provider (PostgreSQL, SQL Server, etc.)

