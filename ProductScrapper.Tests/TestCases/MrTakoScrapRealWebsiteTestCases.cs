namespace ProductScrapper.Tests.TestCases;

using AngleSharp.Html.Parser;

using Itadakimasu.Core.Tests;

using ProductScrapper.Contracts;
using ProductScrapper.Services;
using ProductScrapper.Tests.TestData;

public class MrTakoScrapRealWebsiteTestCases : TestCases<ExpectedProducts, ScrappingInputData>
{
    /// <inheritdoc />
    protected override IEnumerable<TestCase<ExpectedProducts, ScrappingInputData>> GetTestCases()
    {
        return new[]
        {
            GetSuccessScrappingTestCase()
        };
    }

    private static IProductWebSiteScrapper GetScrapper()
    {
        var settings = new ScrappingSettings
        {
            ScrappingRestaurantUrls = new[]
            {
                "https://mistertako.ru/menyu/wok",
                "https://mistertako.ru/menyu/wok?page=2",
                "https://mistertako.ru/menyu/bluda-fri",
                "https://mistertako.ru/menyu/deserty",
                "https://mistertako.ru/menyu/dobavki",
                "https://mistertako.ru/menyu/napitki",
                "https://mistertako.ru/menyu/osnovnye-bluda",
                "https://mistertako.ru/menyu/rolly",
                "https://mistertako.ru/menyu/rolly?page=2",
                "https://mistertako.ru/menyu/salaty",
                "https://mistertako.ru/menyu/supy"
            }
        };
        var htmlParser = new HtmlParser();
        var productParser = new MrTakoParser(htmlParser);
        var httpClient = new HttpClient();
        var scrapper = new MrTakoScrapper(httpClient, settings, productParser);

        return scrapper;
    }

    private static ExpectedProducts GetSuccessScrappingExpected()
    {
        var scrappedProducts = TestInputData.GetMrTakoScrappedProducts();

        return new ExpectedProducts
        {
            ScrappedProducts = scrappedProducts
        };
    }

    private static ScrappingInputData GetSuccessScrappingInputData()
    {
        var scrapper = GetScrapper();

        return new ScrappingInputData
        {
            Scrapper = scrapper,
        };
    }

    private static MrTakoScrappingTestCase GetSuccessScrappingTestCase()
    {
        var inputData = GetSuccessScrappingInputData();
        var expected = GetSuccessScrappingExpected();

        return new MrTakoScrappingTestCase
        {
            Expected = expected,
            InputData = inputData,
            TestCaseName = nameof(GetSuccessScrappingTestCase)
        };
    }

    private record MrTakoScrappingTestCase : ScrappingTestCase;
}