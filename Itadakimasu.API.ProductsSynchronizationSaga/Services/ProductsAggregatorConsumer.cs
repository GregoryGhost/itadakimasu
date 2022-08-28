namespace Itadakimasu.API.ProductsSynchronizationSaga.Services;

using JetBrains.Annotations;

using MassTransit;

using ProductsProxy.V1;

[UsedImplicitly]
public class ProductsAggregatorConsumer : IConsumer<SynchronizationRestaurantRequest>
{
    private readonly ProductsProxy.ProductsProxyClient _aggregatorProductsClient;

    public ProductsAggregatorConsumer(ProductsProxy.ProductsProxyClient aggregatorProductsClient)
    {
        _aggregatorProductsClient = aggregatorProductsClient;
    }

    public async Task Consume(ConsumeContext<SynchronizationRestaurantRequest> context)
    {
        var response = await _aggregatorProductsClient.CreateSynchronizationRestaurantRequestAsync(context.Message);
        await context.RespondAsync<CreatedSynchronizationRestaurantRequest>(response);
    }
}