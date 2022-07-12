namespace ProductScrapper;

using AngleSharp.Html.Parser;

public class MrTakoScrapper: IProductScrapper
{
    private readonly HttpClient _httpClient;
    
    private readonly ScrappingSettings _scrappingSettings;

    private readonly HtmlParser _htmlParser;

    public MrTakoScrapper(HttpClient httpClient, ScrappingSettings scrappingSettings, HtmlParser htmlParser)
    {
        _httpClient = httpClient;
        _scrappingSettings = scrappingSettings;
        _htmlParser = htmlParser;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ScrappedProduct>> ScrapProductsAsync()
    {
        var scrappedProductsTasks = _scrappingSettings.ScrappingRestaurantUrls
                                                 .Select(async scrappingUrl => await ScrapSerialProductsAsync(scrappingUrl));
        var scrappedProducts = (await Task.WhenAll(scrappedProductsTasks))
                               .SelectMany(x => x)
                               .ToList();

        return scrappedProducts;
    }

    private async Task<IEnumerable<ScrappedProduct>> ScrapSerialProductsAsync(string scrappingUrl)
    {
        var response = await _httpClient.GetAsync(scrappingUrl);
        if (!response.IsSuccessStatusCode)
        {
            //TODO: replace on error result
            return Enumerable.Empty<ScrappedProduct>();
        }
        
        var htmlSourceCode = await response.Content.ReadAsStringAsync();
        
        var document = await _htmlParser.ParseDocumentAsync(htmlSourceCode);
        var parsedProductNames = document.QuerySelectorAll(".cat-item-content-T")
                                         .Select(x => x.InnerHtml)
                                         .ToList();
        var parsedProductPrices = document.QuerySelectorAll("span")
                                          .Where(x => x.Attributes.FirstOrDefault(y => y.Name == "itemprop" && y.Value == "price") is not null)
                                          .Select(x => x.InnerHtml)
                                          .ToList();
        var isNotEqualCountNamesAndPrices = parsedProductNames.Count != parsedProductPrices.Count;
        if (isNotEqualCountNamesAndPrices)
        {
            //TODO: replace on error result
            return Enumerable.Empty<ScrappedProduct>();
        }

        var isEmptyProductNames = !parsedProductNames.Any();
        if (isEmptyProductNames)
        {
            //TODO: replace on error result
            return Enumerable.Empty<ScrappedProduct>();
        }
        
        var isEmptyProductPrices = !parsedProductPrices.Any();
        if (isEmptyProductPrices)
        {
            //TODO: replace on error result
            return Enumerable.Empty<ScrappedProduct>();
        }

        var parsedProductInfos = parsedProductNames.Zip(parsedProductPrices)
                                                   .Select(
                                                       sourceProductInfo =>
                                                       {
                                                           var sourceProductName = sourceProductInfo.First;
                                                           var sourceProductPrice = sourceProductInfo.Second;

                                                           var parsedProductPrice = decimal.TryParse(
                                                               sourceProductPrice,
                                                               out var price)
                                                               ? price
                                                               : (decimal?) null;

                                                           return (ProductName: sourceProductName,
                                                               ProductPrice: parsedProductPrice);
                                                       })
                                                   .Where(
                                                       x => !string.IsNullOrWhiteSpace(x.ProductName) &&
                                                            x.ProductPrice is not null)
                                                   .Select(x => new ScrappedProduct
                                                   {
                                                       Name = x.ProductName,
                                                       Price = x.ProductPrice ?? throw new Exception("Product price have no value.")
                                                   });

        return parsedProductInfos;
    }
}

public record ScrappingSettings
{
    public IReadOnlyList<string> ScrappingRestaurantUrls { get; init; } = Array.Empty<string>();
}