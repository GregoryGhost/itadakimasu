namespace ProductScrapper.Tests.TestCases;

using AngleSharp.Html.Parser;

public class MrTakoScrapRealWebsiteTestCases 
{
    public void Test()
    {
        var httpClient = new HttpClient();
        var settings = new ScrappingSettings
        {
            ScrappingRestaurantUrls = new []
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
        IProductWebSiteScrapper scrapper = new MrTakoScrapper(httpClient, settings, productParser);
    }
}