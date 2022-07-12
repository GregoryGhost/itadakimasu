namespace ProductScrapper.Tests.TestCases;

public record ExpectedProducts
{
    public IReadOnlyList<ScrappedProduct> ScrappedProducts { get; init; } = Array.Empty<ScrappedProduct>();
}