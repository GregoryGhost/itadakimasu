namespace ProductScrapper.Tests.TestCases;

using AngleSharp.Html.Parser;

using Itadakimasu.Core.Tests;

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
        //TODO:
        var scrappedProducts = new [] { new ScrappedProduct { Name = "Булочка «Баоцзы» 1шт", Price = 19 }, new ScrappedProduct { Name = "Даньхуатан", Price = 210 }, new ScrappedProduct { Name = "Мисоширу", Price = 210 }, new ScrappedProduct { Name = "Окрошка азиатская", Price = 240 }, new ScrappedProduct { Name = "Рамен с курицей \"Терияки\"", Price = 200 }, new ScrappedProduct { Name = "Рамен со свининой", Price = 210 }, new ScrappedProduct { Name = "Рамен сырный", Price = 280 }, new ScrappedProduct { Name = "Сливочный рамен с курицей и шампиньонами", Price = 220 }, new ScrappedProduct { Name = "Том ям кай NEW", Price = 270 }, new ScrappedProduct { Name = "Том ям с креветками и кальмаром NEW", Price = 350 }};
        
        return new ExpectedProducts
        {
            ScrappedProducts = scrappedProducts
        };
    }

    private static ParsingInputData GetSuccessParsingInputData()
    {
        var pathToTestHtml = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData/Супы.html");
        var html = File.ReadAllText(pathToTestHtml);

        // ReSharper disable once SuspiciousTypeConversion.Global
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
}

public record MrTakoParsingTestCase: TestCase<ExpectedProducts, ParsingInputData> {}