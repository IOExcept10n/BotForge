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
    public override StateResult OnModuleRoot(SelectionStateContext ctx)
    {
        // Gets actual counter value.
        // If counter has not been deifned (transition to root state is always without data)
        if (!ctx.TryGetData(out int counter))
            counter = 0;

        // You can react on selected buttons using switch operator.
        return ctx.Selection() switch
        {
            nameof(Labels.ShowCounter) => RetryWithMessage(ctx, GetCounterStats(counter)),
            nameof(Labels.IncrementCounter) => RetryWith(ctx, ++counter, "✅ Counter incremented!\n\n" + GetCounterStats(counter)),
            nameof(Labels.ResetCounter) => RetryWith(ctx, 0, "🔄 Counter reset to 0!"),
            _ => InvalidInput(ctx) // Basically unreachable
        };
    }

    /// <summary>
    /// Helper method to get the current counter stats string from counter value.
    /// </summary>
    private static string GetCounterStats(int counter) => $"📊 Current value: {counter}\n\n💾 This value is stored in the database and will persist across bot restarts!";
}
