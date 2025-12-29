using System.Drawing;

namespace telegrambot.Models;

internal abstract record ElectronicsItem(string Name, decimal Price, IEnumerable<string> Features, IEnumerable<KnownColor> Colors) : Item(Name, Price);
internal record Smartphone(string Name, decimal Price, IEnumerable<string> Features, IEnumerable<KnownColor> Colors) : ElectronicsItem(Name, Price, Features, Colors);
internal record Headphones(string Name, decimal Price, IEnumerable<string> Features, IEnumerable<KnownColor> Colors) : ElectronicsItem(Name, Price, Features, Colors);
internal record Smartwatch(string Name, decimal Price, IEnumerable<string> Features, IEnumerable<KnownColor> Colors) : ElectronicsItem(Name, Price, Features, Colors);
