namespace ProductScrapper.Tests.TestCases;

public record ScrappingInputData
{
    public IProductScrapper<object> Scrapper { get; init; } = null!;
}