namespace ProductScrapper.Contracts;

using JetBrains.Annotations;

[PublicAPI]
public record ScrappedProduct
{
    public string Name { get; init; } = null!;

    public decimal Price { get; init; }
}