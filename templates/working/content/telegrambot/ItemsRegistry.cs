using System.Drawing;
using telegrambot.Models;

namespace telegrambot;

internal class ItemsRegistry
{
    public IEnumerable<Item> Items { get; } =
    [
        // Electronics
        new Smartphone(
            "XPhone 12",
            799m,
            ["128GB storage", "12MP camera"],
            [KnownColor.Black, KnownColor.White, KnownColor.Blue]
        ),
        new Smartphone(
            "ZPhone Pro",
            999m,
            ["256GB storage", "108MP camera", "5G option"],
            [KnownColor.Silver, KnownColor.Gold, KnownColor.Black]
        ),
        new Headphones(
            "Noise-Canceling Headphones",
            299m,
            ["Wireless", "Up to 20 hours of battery"],
            [KnownColor.Black, KnownColor.White]
        ),
        new Headphones(
            "Over-Ear Headphones",
            199m,
            ["Hi-Fi sound", "Wired"],
            [KnownColor.Blue, KnownColor.Green]
        ),
        new Smartwatch(
            "HealthWatch Pro",
            349m,
            ["Heart rate monitor", "GPS", "Waterproof"],
            [KnownColor.MistyRose, KnownColor.Black]
        ),
        new Smartwatch(
            "FitnessBand 2",
            149m,
            ["Step tracking", "Sleep monitoring"],
            [KnownColor.Red, KnownColor.Blue]
        ),

        // Clothing
        new TShirt(
            "Graphic T-Shirt",
            25m,
            ["Graphic", "Solid" ],
            [Sizes.S, Sizes.M, Sizes.L, Sizes.XL ]
        ),
        new Jeans(
            "Slim Fit Jeans",
            50m,
            ["Slim Fit"],
            [Sizes.S, Sizes.M]
        ),
        new Dress(
            "Casual Summer Dress",
            45m,
            ["Casual", "Formal"],
            [Sizes.XS, Sizes.S, Sizes.M, Sizes.L ]
        ),
        new Activewear(
            "Luxe Leggings",
            40m,
            ["High-waisted", "Compression"],
            [Sizes.S, Sizes.M, Sizes.L ]
        ),

        // Home Goods
        new KitchenAppliance(
            "Blender",
            120m,
            ["1200 Watts", "6 Speeds"],
            [KnownColor.Black, KnownColor.Red]
        ),
        new KitchenAppliance(
            "Coffee Maker",
            89m,
            ["12-Cup", "Programmable"],
            [KnownColor.SteelBlue, KnownColor.Black]
        ),
        new Furniture(
            "Sectional Sofa",
            999m,
            ["Comfortable", "Leather options"],
            [KnownColor.Gray, KnownColor.Beige]
        ),
        new Bedding(
            "Twin Comforter Set",
            70m,
            ["Soft", "Breathable"],
            [KnownColor.White, KnownColor.Blue]
        )
    ];
}
