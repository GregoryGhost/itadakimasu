namespace ProductScrapper;

using JetBrains.Annotations;

// public interface IProductScrapper
// {
//     Result<IEnumerable<ScrappedProduct>, ScrappedError> ScrapProducts();
// }
//
// public record ScrappedProduct

[PublicAPI]
public interface IProductScrapper<TParsingSourceData>
{
    IProductParser<TParsingSourceData> ProductParser { get; init; }

    ScrappingSettings ScrappingSettings { get; init; }

    async Task<IEnumerable<ScrappedProduct>> ScrapProductsAsync()
    {
        var scrappedProductsTasks = ScrappingSettings.ScrappingRestaurantUrls
                                                     .Select(async scrappingUrl => await ScrapSerialProductsAsync(scrappingUrl));
        var scrappedProducts = (await Task.WhenAll(scrappedProductsTasks))
                               .SelectMany(x => x)
                               .ToList();

        return scrappedProducts;
    }

    Task<IEnumerable<ScrappedProduct>> ScrapSerialProductsAsync(string scrappingUrl);
}

[PublicAPI]
public interface IProductHtmlParser : IProductParser<string>
{
}

[PublicAPI]
public interface IProductWebSiteScrapper : IProductScrapper<string>
{
}

[PublicAPI]
public abstract class ProductWebSiteScrapper : IProductWebSiteScrapper
{
    private readonly HttpClient _httpClient;

    protected ProductWebSiteScrapper(ScrappingSettings scrappingSettings, IProductHtmlParser productParser, HttpClient httpClient)
    {
        _httpClient = httpClient;
        ScrappingSettings = scrappingSettings;
        ProductParser = productParser;
    }

    public IProductParser<string> ProductParser { get; init; }

    /// <inheritdoc />
    public ScrappingSettings ScrappingSettings { get; init; }

    /// <inheritdoc />
    public async Task<IEnumerable<ScrappedProduct>> ScrapSerialProductsAsync(string scrappingUrl)
    {
        var response = await _httpClient.GetAsync(scrappingUrl);
        if (!response.IsSuccessStatusCode)
            //TODO: replace on error result
            return Enumerable.Empty<ScrappedProduct>();

        var htmlSourceCode = await response.Content.ReadAsStringAsync();
        var parsedProducts = await ProductParser.ParseProductsAsync(htmlSourceCode);

        return parsedProducts;
    }
}

[PublicAPI]
public interface IProductParser<in TSourceData>
{
    Task<IEnumerable<ScrappedProduct>> ParseProductsAsync(TSourceData sourceData);
}

[PublicAPI]
public record ScrappedProduct
{
    public string Name { get; init; } = null!;

    public decimal Price { get; init; }
}