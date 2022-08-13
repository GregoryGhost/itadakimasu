namespace Itadakimasu.API.ProductsAggregator.Services;

using System.Runtime.CompilerServices;

using DtoTypes;

using Itadakimasu.API.ProductsAggregator.Models;
using Itadakimasu.ProductsAggregator.DAL;

using ProductScrapper.Contracts;

using ProductsProxy.V1;

using Z.EntityFramework.Plus;

public class ProductsSynchronizationNotifier
{
    private readonly ProductsResultSynchronizationReader _productsResultSynchronizationReader;

    private readonly AppDbContext _dbContext;

    private readonly ILogger<ProductsSynchronizationNotifier> _logger;

    public ProductsSynchronizationNotifier(ProductsResultSynchronizationReader productsResultSynchronizationReader, AppDbContext dbContext, ILogger<ProductsSynchronizationNotifier> logger)
    {
        _productsResultSynchronizationReader = productsResultSynchronizationReader;
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async IAsyncEnumerable<SynchronizedRestaurantProductsRequest> GetNotificationAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var scrappedResult in _productsResultSynchronizationReader.ReadAllAsync(cancellationToken))
        {
            var savedProductsRequest = await TrySaveProductsSynchronizationResultAsync(scrappedResult, cancellationToken);
            if (savedProductsRequest is null)
                continue;
            
            var synchronizedRequest = MapSynchronizedRequest(savedProductsRequest);
                
            yield return synchronizedRequest;
        }
    }
    
    private static SynchronizedRestaurantProductsRequest MapSynchronizedRequest(SavedProductsRequest savedProductsRequest)
    {
        var list = savedProductsRequest.SavedProducts.Select(x => new ProductDto
        {
            Id = (ulong)x.Id,
            Name = x.Name,
            Price = x.Price,
            RestaurantId = (ulong)x.RestaurantId
        });
        var mapped = new SynchronizedRestaurantProductsRequest
        {
            Products = {list},
            RequestId = (ulong)savedProductsRequest.SynchronizingRequestId
        };

        return mapped;
    }

    private async Task<SavedProductsRequest?> TrySaveProductsSynchronizationResultAsync(SynchronizingScrappedResult scrappedResult,
        CancellationToken cancellationToken)
    {
        var syncRequestId = new object?[] {scrappedResult.SynchronizingRequestId};
        var synchronizingRequest = await _dbContext.SynchronizatingRestaurants.FindAsync(
            syncRequestId,
            cancellationToken: cancellationToken);
        if (synchronizingRequest is null)
        {
            _logger.LogError("Not found synchronizing restaurant request by id {requestId}", scrappedResult.SynchronizingRequestId);
            
            return null;
        }

        synchronizingRequest.EndSynchronization = DateTime.Now;
        synchronizingRequest.ScrappingErrors = scrappedResult.ScrappedResults.Errors.GetScrappingErrors();
        var hasScrappingErrors = scrappedResult.ScrappedResults.Errors.Any();
        if (hasScrappingErrors)
        {
            return null;
        }

        await _dbContext.Products.DeleteAsync(cancellationToken: cancellationToken);

        var newProducts = scrappedResult.ScrappedResults.ScrappedProducts.Select(
            x => new Product
            {
                Name = x.Name,
                Price = x.Price,
                Restaurant = synchronizingRequest.Restaurant,
            }).ToList();
        await _dbContext.AddRangeAsync(newProducts, cancellationToken);

        var result = new SavedProductsRequest
        {
            SavedProducts = newProducts,
            SynchronizingRequestId = scrappedResult.SynchronizingRequestId
        };

        return result;
    }

    private record SavedProductsRequest
    {
        public IEnumerable<Product> SavedProducts { get; init; } = Enumerable.Empty<Product>();
        public long SynchronizingRequestId { get; init; }
    }
}