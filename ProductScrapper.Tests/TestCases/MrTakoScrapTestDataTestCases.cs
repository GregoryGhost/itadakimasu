namespace ProductScrapper.Tests.TestCases;

using System.Net;

using AngleSharp.Html.Parser;

using Itadakimasu.Core.Tests;

using ProductScrapper.Tests.TestData;

public class MrTakoScrapTestDataTestCases : TestCases<ExpectedProducts, ScrappingInputData>
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
                "https://mistertako.ru/test-menu"
            }
        };
        var htmlParser = new HtmlParser();
        var productParser = new MrTakoParser(htmlParser);
        var httpClient = new MockedHttpClient();
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

    private class MockedHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(BakedHttpResponseMessage());
        }

        private static HttpResponseMessage BakedHttpResponseMessage()
        {
            var html = TestInputData.GetMrTakoWebSiteData();
            var httpResponse = new HttpResponseMessage
            {
                Content = new StringContent(html),
                StatusCode = HttpStatusCode.OK,
            };

            return httpResponse;
        }
    }

    private class MockedHttpClient : HttpClient
    {
        private static readonly MockedHttpMessageHandler HttpMessageHandler = new();

        public MockedHttpClient()
            : base(HttpMessageHandler)
        {
        }
    }
}