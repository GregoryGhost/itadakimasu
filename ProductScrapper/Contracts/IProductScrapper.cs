namespace ProductScrapper.Contracts;

using JetBrains.Annotations;

using ProductScrapper.Services;

[PublicAPI]
public interface IProductScrapper<TParsingSourceData>
{
    IProductParser<TParsingSourceData> ProductParser { get; init; }

    ScrappingSettings ScrappingSettings { get; init; }

    async Task<ScrappingResults> ScrapProductsAsync()
    {
        var scrappedProductsTasks = ScrappingSettings.ScrappingRestaurantUrls
                                                     .Select(async scrappingUrl => await ScrapSerialProductsAsync(scrappingUrl));
        var groupedResults = (await Task.WhenAll(scrappedProductsTasks))
                               .Select(x => x.Result)
                               .ToList();
        var scrappedErrors = groupedResults.Where(x => x.IsFailure)
                                           .Select(x => x.Error)
                                           .ToList();
        var scrappedProducts = groupedResults.Where(x => x.IsSuccess)
                                             .SelectMany(x => x.Value)
                                             .ToList();

        var results = new ScrappingResults
        {
            Errors = scrappedErrors,
            ScrappedProducts = scrappedProducts
        };

        return results;
    }

    Task<ScrappingResult> ScrapSerialProductsAsync(string scrappingUrl);
}