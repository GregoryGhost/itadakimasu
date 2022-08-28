namespace Itadakimasu.API.ProductsSynchronizationSaga.Services;

using Grpc.Core;

using JetBrains.Annotations;

using MassTransit;

using ProductsSyncer.V1;

[UsedImplicitly]
public class ProductsSyncConsumer : IConsumer<SynchronizingData>
{
    private readonly ProductsSyncer.ProductsSyncerClient _productsSyncer;

    private readonly IBus _bus;

    public ProductsSyncConsumer(ProductsSyncer.ProductsSyncerClient productsSyncer, IBus bus)
    {
        _productsSyncer = productsSyncer;
        _bus = bus;
    }

    public async Task Consume(ConsumeContext<SynchronizingData> context)
    {
        using var call = _productsSyncer.Synchronize();
            
        await WriteAsync(call, context.Message);
        await ReadAsync(call);
            
        await call.RequestStream.CompleteAsync();
    }

    private async Task ReadAsync(AsyncDuplexStreamingCall<SynchronizingData, SynchronizedData> call)
    {
        await foreach (var response in call.ResponseStream.ReadAllAsync())
        {
            await _bus.Publish(response);
        }
    }

    private static async Task WriteAsync(AsyncDuplexStreamingCall<SynchronizingData, SynchronizedData> call,
        SynchronizingData synchronizingData)
    {
        await call.RequestStream.WriteAsync(synchronizingData);
    }
}