namespace ProductScrapper.Tests.TestCases;

public record ParsingInputData
{
    public IProductHtmlParser Parser { get; init; } = null!;

    public string SourceParsingData { get; init; } = null!;
}