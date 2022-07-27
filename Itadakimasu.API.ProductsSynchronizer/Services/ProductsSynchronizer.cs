namespace Itadakimasu.API.ProductsSynchronizer.Services;

using ProductsSyncer.V1;

using Grpc.Core;

using Merchandiser.V1;

public class ProductsSynchronizer : ProductsSyncer.ProductsSyncerBase
{
    private readonly ILogger<ProductsSynchronizer> _logger;

    private readonly Merchandiser.MerchandiserClient _productsClient;

    public ProductsSynchronizer(ILogger<ProductsSynchronizer> logger, Merchandiser.MerchandiserClient productsClient)
    {
        _logger = logger;
        _productsClient = productsClient;
    }

    /// <inheritdoc />
    public override async Task Synchronize(IAsyncStreamReader<SynchronizingData> requestStream, IServerStreamWriter<SynchronizedData> responseStream, ServerCallContext context)
    {
        await foreach (var synchronizingData in requestStream.ReadAllAsync(context.CancellationToken))
        {
            await BatchCreateProducts(synchronizingData);
            await NotifySynchronizationEnd(responseStream, synchronizingData);
        }
    }

    private static async Task NotifySynchronizationEnd(IServerStreamWriter<SynchronizedData> responseStream, SynchronizingData synchronizingData)
    {
        var synchronizedData = new SynchronizedData
        {
            RequestId = synchronizingData.RequestId
        };
        await responseStream.WriteAsync(synchronizedData);
    }

    private async Task BatchCreateProducts(SynchronizingData synchronizingData)
    {
        var dtos = ConvertToBatchData(synchronizingData);
        await _productsClient.BatchCreateProductsAsync(dtos);
    }

    private static BatchProductsDto ConvertToBatchData(SynchronizingData data)
    {
        var products = data.Products.Select(
            x => new DtoTypes.ProductDto
            {
                Id = x.Id,
                Name = x.Name,
                Price = x.Price,
                RestaurantId = x.RestaurantId,
            });
        var dto = new BatchProductsDto
        {
            Products = {products}
        };

        return dto;
    }
}