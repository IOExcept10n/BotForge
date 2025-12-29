using BotForge.Messaging;

namespace telegrambot;

[LabelStorage]
internal static class Labels
{
    public static ButtonLabel Activewear => new(Emoji.RunningShirtWithSash);
    public static ButtonLabel Bedding => new(Emoji.CrescentMoon);
    public static ButtonLabel Clothing => new(Emoji.WomansHat);
    public static ButtonLabel Dresses => new(Emoji.Dress);
    public static ButtonLabel Electronics => new(Emoji.ElectricPlug);
    public static ButtonLabel Furniture => new(Emoji.HouseBuilding);
    public static ButtonLabel Headphones => new(Emoji.Headphone);
    public static ButtonLabel HomeGoods => new(Emoji.HouseWithGarden);
    public static ButtonLabel KitchenAppliances => new(Emoji.ForkAndKnife);
    public static ButtonLabel MensClothing => new(Emoji.Man);
    public static ButtonLabel Smartphones => new(Emoji.MobilePhone);
    public static ButtonLabel Smartwatches => new(Emoji.Watch);
    public static ButtonLabel TShirts => new(Emoji.TShirt);
    public static ButtonLabel WomensClothing => new(Emoji.Woman);
}
