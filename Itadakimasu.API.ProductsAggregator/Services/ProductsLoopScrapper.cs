namespace Itadakimasu.API.ProductsAggregator.Services;

using Itadakimasu.API.ProductsAggregator.Models;
using Itadakimasu.ProductsAggregator.DAL;

using JetBrains.Annotations;

using ProductScrapper.Contracts;

[UsedImplicitly]
public class ProductsLoopScrapper : IHostedService
{
    private readonly ILogger<ProductsLoopScrapper> _logger;

    private readonly ProductsResultSynchronizationWriter _resultWriter;

    private readonly ProductsAggregatorScrapper _scrapper;

    private readonly ProductsSynchronizationReader _synchronizationReader;

    public ProductsLoopScrapper(ProductsSynchronizationReader synchronizationReader, ProductsAggregatorScrapper scrapper,
        ProductsResultSynchronizationWriter resultWriter, ILogger<ProductsLoopScrapper> logger)
    {
        _synchronizationReader = synchronizationReader;
        _scrapper = scrapper;
        _resultWriter = resultWriter;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await foreach (var request in _synchronizationReader.ChannelReader.ReadAllAsync(cancellationToken))
        {
            if (TryGetScrapperType(request, out var scrapperType))
                continue;

            var scrappedResults = await _scrapper.ScrapAsync(scrapperType);
            var synchronizingScrappedResult = new SynchronizingScrappedResult
            {
                ScrappedResults = scrappedResults,
                SynchronizingRequestId = request.Id
            };
            await _resultWriter.ChannelWriter.WriteAsync(synchronizingScrappedResult, cancellationToken);
        }
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return _synchronizationReader.ChannelReader.Completion;
    }

    private bool TryGetScrapperType(SynchronizatingRestaurant request, out ProductsScrapperType scrapperType)
    {
        var scrapperName = request.Restaurant.RestaurantScrapper.ScrapperName;
        var isProductScapperType = Enum.TryParse(scrapperName, out scrapperType);
        if (isProductScapperType)
            return false;
        
        _logger.LogError("Cannot parse product scrapper type for {scrapperName}", scrapperName);
        
        return true;
    }
}