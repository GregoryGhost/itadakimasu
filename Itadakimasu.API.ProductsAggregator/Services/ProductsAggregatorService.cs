namespace Itadakimasu.API.ProductsAggregator.Services;

using Grpc.Core;

using Itadakimasu.ProductsAggregator.DAL;

using ProductsProxy.V1;

/// <summary>
/// Products aggregator which aggregate product infos from different data sources to unified data format.
/// </summary>
public class ProductsAggregatorService: ProductsProxy.ProductsProxyBase
{
    private readonly AppDbContext _dbContext;

    private readonly ILogger<ProductsAggregatorService> _logger;

    /// <summary>
    /// Initialize depedencies.
    /// </summary>
    /// <param name="logger">Logger service.</param>
    /// <param name="dbContext">Database context.</param>
    public ProductsAggregatorService(AppDbContext dbContext, ILogger<ProductsAggregatorService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <inheritdoc />
    public override Task<PaginatedProducts> ListProducts(ProductsPagination request, ServerCallContext context)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override Task<CreatedSynchronizationRestaurantRequest> CreateSynchronizationRestaurantRequest(SynchronizationRestaurantRequest request, ServerCallContext context)
    {
        throw new NotImplementedException();
    }
}