namespace Itadakimasu.API.ProductsAggregator.Services;

using Itadakimasu.API.ProductsAggregator.Models;

using ProductScrapper.Contracts;

public class ProductsAggregatorScrapper
{
    private readonly IReadOnlyDictionary<ProductsScrapperType, IProductWebSiteScrapper> _scrappers;

    private readonly ILogger<ProductsAggregatorScrapper> _logger;

    private readonly ScrappedResults _mismatchedScrapperTypeResult;

    public ProductsAggregatorScrapper(AvailableScrappers availableScrappers, ILogger<ProductsAggregatorScrapper> logger)
    {
        _logger = logger;
        _scrappers = availableScrappers.Scrappers.ToDictionary(x => x.ScrappingSettings.ProductsScrapperType, x => x);
        _mismatchedScrapperTypeResult = new ScrappedResults
        {
            Errors = Enumerable.Empty<ScrappingErrors>(),
            ScrappedProducts = Enumerable.Empty<ScrappedProduct>()
        };
    }

    public async Task<ScrappedResults> ScrapAsync(ProductsScrapperType scrapperType)
    {
        if (_scrappers.TryGetValue(scrapperType, out var scrapper))
        {
            var scrappedResults = await scrapper.ScrapProductsAsync();
            
            return scrappedResults;
        }
        
        _logger.LogError("Cannot find scrapper by scrapper type {scrapperType}", scrapperType);

        return _mismatchedScrapperTypeResult;
    }
}