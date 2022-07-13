namespace ProductScrapper.Contracts;

using JetBrains.Annotations;

using ProductScrapper.Services;

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
    public async Task<ScrappingResult> ScrapSerialProductsAsync(string scrappingUrl)
    {
        var response = await _httpClient.GetAsync(scrappingUrl);
        if (!response.IsSuccessStatusCode)
            return ScrappingErrors.ResponseError;

        var htmlSourceCode = await response.Content.ReadAsStringAsync();
        var parsedProducts = await ProductParser.ParseProductsAsync(htmlSourceCode);

        return parsedProducts;
    }
}