using BotForge.Fsm;
using BotForge.Modules;
using BotForge.Modules.Attributes;
using BotForge.Modules.Contexts;

namespace BotForge.Telegram.MemoryBot;

/// <summary>
/// A simple module that demonstrates persistence by tracking a counter value.
/// The counter persists across bot restarts thanks to the persistence layer.
/// The state data is automatically persisted by BotForge's persistence system.
/// </summary>
internal class MemoryModule : ModuleBase
{
    /// <summary>
    /// Root state handler - shows welcome message and current counter.
    /// </summary>
    [MenuItem(nameof(Labels.ShowCounter))]
    [MenuItem(nameof(Labels.IncrementCounter))]
    [MenuItem(nameof(Labels.ResetCounter))]
    public override StateResult OnModuleRoot(SelectionStateContext ctx) => ctx.Selection() switch
    {
        nameof(Labels.ShowCounter) => ShowCounter(ctx),
        nameof(Labels.IncrementCounter) => IncrementCounter(ctx),
        nameof(Labels.ResetCounter) => ResetCounter(ctx),
        _ => InvalidInput(ctx) // Basically unreachable
    };

    /// <summary>
    /// Helper method to get the current counter value from state data.
    /// </summary>
    private static int GetCounter(SelectionStateContext ctx) => ctx.TryGetData(out int counter) ? counter : 0;

    /// <summary>
    /// Increments the counter and saves it to persistent storage.
    /// </summary>
    private static StateResult IncrementCounter(SelectionStateContext ctx)
    {
        // Get current counter from persisted state data
        int counter = GetCounter(ctx);
        counter++;

        // Save the new counter value - this will be persisted automatically
        return RetryWith(ctx, counter,
            $"✅ Counter incremented!\n\n📊 Current value: {counter}\n\n" +
            "💾 This value is stored in the database and will persist across bot restarts!");
    }

    /// <summary>
    /// Resets the counter to zero.
    /// </summary>
    private static StateResult ResetCounter(SelectionStateContext ctx)
    {
        return RetryWith(ctx, 0, "🔄 Counter reset to 0!");
    }

    /// <summary>
    /// Shows the current counter value.
    /// </summary>
    private static StateResult ShowCounter(SelectionStateContext ctx)
    {
        int counter = GetCounter(ctx);

        return RetryWithMessage(ctx,
            $"📊 Current counter value: {counter}\n\n" +
            "💾 This value is stored in the database and persists across restarts!\n\n" +
            "🔄 Try restarting the bot - your counter will still be here!");
    }
}
