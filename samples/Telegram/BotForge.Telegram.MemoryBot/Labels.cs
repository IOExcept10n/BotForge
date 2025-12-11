using BotForge.Messaging;

namespace BotForge.Telegram.MemoryBot;

[LabelStorage]
internal static class Labels
{
    public static ButtonLabel IncrementCounter => new(Emoji.HeavyPlusSign, "Increment Counter");
    public static ButtonLabel ResetCounter => new(Emoji.AnticlockwiseDownwardsAndUpwardsOpenCircleArrows, "Reset Counter");
    public static ButtonLabel ShowCounter => new(Emoji.BarChart, "Show Counter");
}
