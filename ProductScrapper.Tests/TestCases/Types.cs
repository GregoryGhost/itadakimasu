namespace ProductScrapper.Tests.TestCases;

using Itadakimasu.Core.Tests;

public record ParsingInputData
{
    public IProductHtmlParser Parser { get; init; } = null!;

    public string SourceParsingData { get; init; } = null!;
}

public record ScrappingInputData
{
    public IProductWebSiteScrapper Scrapper { get; init; } = null!;
}

public record ScrappingTestCase : TestCase<ExpectedProducts, ScrappingInputData>
{
}

public record ParsingTestCase : TestCase<ExpectedProducts, ParsingInputData>
{
}

public record ExpectedProducts
{
    public IReadOnlyList<ScrappedProduct> ScrappedProducts { get; init; } = Array.Empty<ScrappedProduct>();
}