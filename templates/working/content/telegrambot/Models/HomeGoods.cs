using System.Drawing;

namespace telegrambot.Models;

internal abstract record HomeGoodsItem(string Name, decimal Price, IEnumerable<string> Features, IEnumerable<KnownColor> Color) : Item(Name, Price);
internal record KitchenAppliance(string Name, decimal Price, IEnumerable<string> Features, IEnumerable<KnownColor> Color) : HomeGoodsItem(Name, Price, Features, Color);
internal record Furniture(string Name, decimal Price, IEnumerable<string> Features, IEnumerable<KnownColor> Color) : HomeGoodsItem(Name, Price, Features, Color);
internal record Bedding(string Name, decimal Price, IEnumerable<string> Features, IEnumerable<KnownColor> Color) : HomeGoodsItem(Name, Price, Features, Color);
