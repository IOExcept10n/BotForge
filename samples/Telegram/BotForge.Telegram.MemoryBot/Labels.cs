using BotForge.Messaging;

namespace BotForge.Telegram.MemoryBot;

/// <summary>
/// This class stores labels for buttons. You can define your own buttons and place them here to reference from your code.
/// These labels are automatically loaded in routing system, so you can add a link to any label in attributes using e.g. <see langword="nameof"/>(<see cref="ShowCounter"/>).
/// </summary>
[LabelStorage]
internal static class Labels
{
    public static ButtonLabel IncrementCounter => new(Emoji.HeavyPlusSign, "Increment Counter");
    public static ButtonLabel ResetCounter => new(Emoji.AnticlockwiseDownwardsAndUpwardsOpenCircleArrows, "Reset Counter");
    public static ButtonLabel ShowCounter => new(Emoji.BarChart, "Show Counter");
}
