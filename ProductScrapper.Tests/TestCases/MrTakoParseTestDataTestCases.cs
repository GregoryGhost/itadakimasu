namespace ProductScrapper.Tests.TestCases;

using AngleSharp.Html.Parser;

using Itadakimasu.Core.Tests;

using ProductScrapper.Tests.TestData;

public class MrTakoParseTestDataTestCases : TestCases<ExpectedProducts, ParsingInputData>
{
    /// <inheritdoc />
    protected override IEnumerable<TestCase<ExpectedProducts, ParsingInputData>> GetTestCases()
    {
        return new[]
        {
            GetSuccessParsingTestCase()
        };
    }

    private static MrTakoParsingTestCase GetSuccessParsingTestCase()
    {
        var inputData = GetSuccessParsingInputData();
        var expected = GetSuccessParsingExpected();

        return new MrTakoParsingTestCase
        {
            Expected = expected,
            InputData = inputData,
            TestCaseName = nameof(GetSuccessParsingTestCase)
        };
    }

    private static ExpectedProducts GetSuccessParsingExpected()
    {
        var scrappedProducts = TestInputData.GetMrTakoScrappedProducts();
        
        return new ExpectedProducts
        {
            ScrappedProducts = scrappedProducts
        };
    }

    private static ParsingInputData GetSuccessParsingInputData()
    {
        var html = TestInputData.GetMrTakoWebSiteData();
        var parser = GetParser();

        return new ParsingInputData
        {
            Parser = parser,
            SourceParsingData = html
        };
    }

    private static IProductHtmlParser GetParser()
    {
        var htmlParser = new HtmlParser();
        var productParser = new MrTakoParser(htmlParser);

        return productParser;
    }

    private record MrTakoParsingTestCase : ParsingTestCase;
}