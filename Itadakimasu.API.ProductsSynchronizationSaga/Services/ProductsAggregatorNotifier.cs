namespace Itadakimasu.API.ProductsSynchronizationSaga.Services;

using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using MassTransit;

using ProductsProxy.V1;

public class ProductsAggregatorNotifier: IHostedService
{
    private readonly IBus _bus;
    
    private readonly ProductsProxy.ProductsProxyClient _aggregatorProductsClient;

    private readonly Empty _empty;

    public ProductsAggregatorNotifier(IBus bus, ProductsProxy.ProductsProxyClient aggregatorProductsClient)
    {
        _bus = bus;
        _aggregatorProductsClient = aggregatorProductsClient;
        _empty = new Empty();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var call = _aggregatorProductsClient.GetSynchronizedRestaurantProductsRequest(_empty);
        
        await foreach (var response in call.ResponseStream.ReadAllAsync(cancellationToken: cancellationToken))
        {
            await _bus.Publish(response, cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}