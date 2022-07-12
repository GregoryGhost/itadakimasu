namespace ProductScrapper.Tests;

using AngleSharp.Html.Parser;

using FluentAssertions;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task TestScrapProducts()
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
        var scrapper = new MrTakoScrapper(httpClient, settings, htmlParser);
        var scrappedProducts = (await scrapper.ScrapProductsAsync()).ToList();

        scrappedProducts.Should().NotBeEmpty();
        var formatted = string.Join(", ", scrappedProducts);
        Console.WriteLine($"Total count: {scrappedProducts.Count}. Scrapped products: {formatted}");
    }
}