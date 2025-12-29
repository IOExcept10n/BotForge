namespace telegrambot.Models;

internal abstract record Clothing(string Name, decimal Price, IEnumerable<string> Styles, IEnumerable<Sizes> Sizes) : Item(Name, Price);
internal abstract record MensClothing(string Name, decimal Price, IEnumerable<string> Styles, IEnumerable<Sizes> Sizes) : Clothing(Name, Price, Styles, Sizes);
internal abstract record WomensClothing(string Name, decimal Price, IEnumerable<string> Styles, IEnumerable<Sizes> Sizes) : Clothing(Name, Price, Styles, Sizes);
internal record TShirt(string Name, decimal Price, IEnumerable<string> Styles, IEnumerable<Sizes> Sizes) : MensClothing(Name, Price, Styles, Sizes);
internal record Jeans(string Name, decimal Price, IEnumerable<string> Styles, IEnumerable<Sizes> Sizes) : MensClothing(Name, Price, Styles, Sizes);
internal record Dress(string Name, decimal Price, IEnumerable<string> Styles, IEnumerable<Sizes> Sizes) : WomensClothing(Name, Price, Styles, Sizes);
internal record Activewear(string Name, decimal Price, IEnumerable<string> Styles, IEnumerable<Sizes> Sizes) : WomensClothing(Name, Price, Styles, Sizes);

internal enum Sizes
{
    XS,
    S,
    M,
    L,
    XL
}
