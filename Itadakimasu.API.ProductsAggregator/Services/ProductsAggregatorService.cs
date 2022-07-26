namespace Itadakimasu.API.ProductsAggregator.Services;

using System.Diagnostics;

using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using Itadakimasu.API.ProductsAggregator.Models;
using Itadakimasu.Core.DAL;
using Itadakimasu.ProductsAggregator.DAL;

using Microsoft.EntityFrameworkCore;

using ProductsProxy.V1;

/// <summary>
///     Products aggregator which aggregate product infos from different data sources to unified data format.
/// </summary>
public class ProductsAggregatorService : ProductsProxy.ProductsProxyBase
{
    private readonly AppDbContext _dbContext;

    private readonly ILogger<ProductsAggregatorService> _logger;

    private readonly ProductsSynchronizationWriter _productsSynchronizationWriter;

    private readonly ProductsSynchronizationNotifier _productsSynchronizationNotifier;

    /// <summary>
    ///     Initialize depedencies.
    /// </summary>
    /// <param name="logger">Logger service.</param>
    /// <param name="dbContext">Database context.</param>
    public ProductsAggregatorService(AppDbContext dbContext, ILogger<ProductsAggregatorService> logger, ProductsSynchronizationWriter productsSynchronizationWriter, ProductsSynchronizationNotifier productsSynchronizationNotifier)
    {
        _dbContext = dbContext;
        _logger = logger;
        _productsSynchronizationWriter = productsSynchronizationWriter;
        _productsSynchronizationNotifier = productsSynchronizationNotifier;
    }

    /// <inheritdoc />
    public override async Task GetSynchronizedRestaurantProductsRequest(Empty request, IServerStreamWriter<SynchronizedRestaurantProductsRequest> responseStream, ServerCallContext context)
    {
        await foreach (var synchronizingData in _productsSynchronizationNotifier.GetNotificationAsync(context.CancellationToken))
        {
            await responseStream.WriteAsync(synchronizingData);
        }
    }

    /// <inheritdoc />
    public override async Task<CreatedSynchronizationRestaurantRequest> CreateSynchronizationRestaurantRequest(
        SynchronizationRestaurantRequest request, ServerCallContext context)
    {
        var (hasFailedCheck, foundRestaurant) = await CheckExistingRestaurantSynchronizationAsync(
            request,
            context);
        if (hasFailedCheck)
            return null!;

        Debug.Assert(foundRestaurant != null, nameof(foundRestaurant) + " != null");
        var (created, newRequest) = await CreateNewSynchronizationRestaurantRequest(foundRestaurant);

        await _productsSynchronizationWriter.WriteAsync(newRequest);

        return created;
    }

    /// <inheritdoc />
    public override async Task<RestaurantProductsStatusSynchronization> GetStatusSynchronization(
        SynchronizationRestaurantId request, ServerCallContext context)
    {
        var foundSynchronizatingRequest = await _dbContext.SynchronizatingRestaurants.FindAsync(request.Id);
        if (foundSynchronizatingRequest is null)
        {
            context.Status = new Status(StatusCode.AlreadyExists, "The request by id was not found.");

            return null!;
        }

        var dto = MapDto(foundSynchronizatingRequest);

        return dto;
    }

    /// <inheritdoc />
    public override async Task<PaginatedProducts> ListProducts(ProductsPagination request, ServerCallContext context)
    {
        var list = await _dbContext.Products
                                   .Paginate(request.Pagination)
                                   .Select(x => MapDto(x))
                                   .ToListAsync();
        var pageInfo = await _dbContext.Products.ToPaginatedAsync(request.Pagination);
        var paginatedProducts = new PaginatedProducts
        {
            PageInfo = pageInfo,
            Products = {list}
        };

        return paginatedProducts;
    }

    /// <inheritdoc />
    public override async Task<PaginatedRestaurants> ListRestaurants(RestaurantsPagination request, ServerCallContext context)
    {
        var list = await _dbContext.Restaurants
                                   .Paginate(request.Pagination)
                                   .Select(x => MapDto(x))
                                   .ToListAsync();
        var pageInfo = await _dbContext.Restaurants.ToPaginatedAsync(request.Pagination);
        var paginatedRestaurants = new PaginatedRestaurants
        {
            PageInfo = pageInfo,
            Restaurants = {list}
        };

        return paginatedRestaurants;
    }

    /// <inheritdoc />
    public override async Task<PaginatedSynchronizationRestaurants> ListStatusSynchronization(
        SynchronizationRestaurantsPagination request, ServerCallContext context)
    {
        var list = await _dbContext.SynchronizatingRestaurants
                                   .Paginate(request.Pagination)
                                   .Select(x => MapDto(x))
                                   .ToListAsync();
        var pageInfo = await _dbContext.SynchronizatingRestaurants.ToPaginatedAsync(request.Pagination);
        var paginated = new PaginatedSynchronizationRestaurants
        {
            PageInfo = pageInfo,
            Restaurants = { list }
        };

        return paginated;
    }

    private async Task<(bool IsFailedCheck, Restaurant? FoundRestaurant)> CheckExistingRestaurantSynchronizationAsync(
        SynchronizationRestaurantRequest request, ServerCallContext context)
    {
        if (request.RestaurantId == 0)
        {
            context.Status = new Status(StatusCode.FailedPrecondition, "The restaurant id must be above zero.");

            return (true, null);
        }

        var foundRestaurant = await _dbContext.Restaurants.FindAsync(request.RestaurantId);
        if (foundRestaurant is null)
        {
            context.Status = new Status(StatusCode.NotFound, "The passed restaurant id was not found.");

            return (true, null);
        }

        var isExistingTheSameRequest = await _dbContext.SynchronizatingRestaurants.FirstOrDefaultAsync(
            x => x.Restaurant.Id == foundRestaurant.Id && x.Status == SynchronizationProductStatus.InProgress);
        if (isExistingTheSameRequest is not null)
            return (false, foundRestaurant);

        context.Status = new Status(StatusCode.AlreadyExists, "The same request for the passed restaurant id is executing.");

        return (true, foundRestaurant);
    }
    
    private async Task<(CreatedSynchronizationRestaurantRequest Created, SynchronizatingRestaurant NewRequest)> CreateNewSynchronizationRestaurantRequest(
        Restaurant foundRestaurant)
    {
        var newRequest = new SynchronizatingRestaurant
        {
            EndSynchronization = null,
            Restaurant = foundRestaurant,
            StartSynchronization = default,
            Status = SynchronizationProductStatus.InProgress,
            ScrappingErrors = string.Empty,
        };
        await _dbContext.SynchronizatingRestaurants.AddAsync(newRequest);
        await _dbContext.SaveChangesAsync();

        var created = new CreatedSynchronizationRestaurantRequest
        {
            RequestId = (ulong) newRequest.Id
        };

        return (created, newRequest);
    }

    private static ProductDto MapDto(Product product)
    {
        var dto = new ProductDto
        {
            Id = (ulong) product.Id,
            Name = product.Name,
            Price = product.Price,
            RestaurantId = (ulong) product.RestaurantId
        };

        return dto;
    }

    private static RestaurantDto MapDto(Restaurant restaurant)
    {
        var dto = new RestaurantDto
        {
            Id = (ulong) restaurant.Id,
            Name = restaurant.Name
        };

        return dto;
    }

    private static RestaurantProductsStatusSynchronization MapDto(SynchronizatingRestaurant request)
    {
        var status = MapSynchronizatingStatus(request.Status);
        var dto = new RestaurantProductsStatusSynchronization
        {
            Id = (ulong) request.Id,
            Status = status,
            StartDate = request.StartSynchronization.ToTimestamp(),
            EndDate = request.EndSynchronization?.ToTimestamp()
        };

        return dto;
    }

    private static SynchronizationRestaurantProductStatuses MapSynchronizatingStatus(SynchronizationProductStatus status)
    {
        if (status == SynchronizationProductStatus.InProgress)
            return SynchronizationRestaurantProductStatuses.InProgress;

        if (status == SynchronizationProductStatus.Done)
            return SynchronizationRestaurantProductStatuses.Done;

        throw new ArgumentOutOfRangeException(nameof(status));
    }
}