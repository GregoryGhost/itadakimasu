namespace Itadakimasu.API.ProductsSynchronizationSaga.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Grpc.Core;

    using Itadakimasu.API.ProductsSynchronizationSaga.Types.Saga;

    using MassTransit;
    

    using Microsoft.Extensions.Logging;

    using ProductsProxy.V1;

    using ProductsSyncer.V1;

    public sealed class ProductsSynchronizationSaga : MassTransitStateMachine<ProductsSynchronizationSagaState>
    {
        private readonly ILogger<ProductsSynchronizationSaga> _logger;


        public ProductsSynchronizationSaga(ILogger<ProductsSynchronizationSaga> logger)
        {
            _logger = logger;
            //TODO: fix handle errors
            InstanceState(x => x.CurrentState);
            Event(() => SynchronizeRestaurantProducts, 
                x => 
                    x.CorrelateBy((instance, context) => instance.RestaurantId == context.Message.RestaurantId)
                     .SelectId(y => NewId.NextGuid()));
            Event(
                () => ScrappedRestaurantProducts,
                x =>
                    x.CorrelateBy((instance, context) => instance.RestaurantId == context.Message.RequestId));
            Event(
                () => GetSynchronizedRestaurant,
                x =>
                    x.CorrelateBy((instance, context) => instance.RestaurantId == context.Message.RequestId));
            Event(
                () => SynchronizeRestaurant,
                x =>
                    x.CorrelateBy((instance, context) => instance.RestaurantId == context.Message.RequestId));

            Initially(

                When(SynchronizeRestaurantProducts)
                    .Then(
                        x =>
                        {
                            if (!x.TryGetPayload(out SagaConsumeContext<ProductsSynchronizationSagaState>? payload))
                                throw new Exception("Unable to retrieve required payload for callback data.");
                            
                            if (payload?.RequestId is null) 
                                throw new Exception("Unable to retrieve required payload request id.");
                            
                            x.Saga.RequestId = payload.RequestId.Value;
                        })
                    .TransitionTo(CreatedScrappingRestaurantProductsRequest)
                // When(CreatedScrappingRestaurantProductsRequest.Faulted)
                //     .ThenAsync(
                //         async context =>
                //         {
                //             await RespondFromSagaAsync(
                //                 context,
                //                 "Faulted On Get Items " + string.Join("; ", context.Data.Exceptions.Select(x => x.Message)));
                //         })
                //     .TransitionTo(Failed),
                //
                // When(GetItems.TimeoutExpired)
                //     .ThenAsync(async context => { await RespondFromSagaAsync(context, "Timeout Expired On Get Items"); })
                //     .TransitionTo(Failed)
            );

            During(
                CreatedScrappingRestaurantProductsRequest,
                When(ScrappedRestaurantProducts)
                    .TransitionTo(SynchronizedRestaurantProducts)
                // When(GetRestaurantProducts.Faulted)
                //     .ThenAsync(
                //         async context =>
                //         {
                //             await RespondFromSagaAsync(
                //                 context,
                //                 "Faulted On Get Money " + string.Join("; ", context.Data.Exceptions.Select(x => x.Message)));
                //         })
                //     .TransitionTo(Failed),
                //
                // When(GetRestaurantProducts.TimeoutExpired)
                //     .ThenAsync(async context => { await RespondFromSagaAsync(context, "Timeout Expired On Get Money"); })
                //     .TransitionTo(Failed)
                );
            
            During(
                SynchronizedRestaurantProducts,
                When(SynchronizeRestaurant)
                    .TransitionTo(SynchronizingRestaurant));
            
            During(
                SynchronizingRestaurant,
                When(GetSynchronizedRestaurant)
                    .TransitionTo(SynchronizedRestaurant)
                );
            
            // During(
            //     SynchronizedRestaurantProducts,
            //     When(SynchronizedRestaurantProducts.Enter)
            //         .ThenAsync(async context => { await RespondFromSagaAsync(context, null); })
            //         .Finalize(),
            //     
            //     When(SynchronizedRestaurantProducts.Faulted)
            //         .ThenAsync(
            //             async context =>
            //             {
            //                 await RespondFromSagaAsync(
            //                     context,
            //                     "Faulted On Get Items " + string.Join("; ", context.Data.Exceptions.Select(x => x.Message)));
            //             })
            //         .TransitionTo(Failed),
            //
            //     When(SynchronizedRestaurantProducts.TimeoutExpired)
            //         .ThenAsync(async context => { await RespondFromSagaAsync(context, "Timeout Expired On Get Items"); })
            //         .TransitionTo(Failed)
            //     );
        }

        public State SynchronizedRestaurantProducts { get; init; } = null!;

        public State CreatedScrappingRestaurantProductsRequest { get; init; } = null!;

        public State Failed { get; init; } = null!;

        public State SynchronizedRestaurant { get; init; } = null!;

        public State SynchronizingRestaurant { get; init; } = null!;

        public Event<SynchronizeRestaurantProductsRequest> SynchronizeRestaurantProducts { get; init; } = null!;

        public Event<SynchronizedRestaurantProductsRequest> ScrappedRestaurantProducts { get; init; } = null!;

        public Event<SynchronizedData> GetSynchronizedRestaurant { get; init; } = null!;

        public Event<SynchronizingData> SynchronizeRestaurant { get; init; } = null!;

        private static async Task RespondFromSagaAsync(SagaConsumeContext<ProductsSynchronizationSagaState> context, string error)
        {
            var endpoint = await context.GetSendEndpoint(context.Saga.ResponseAddress);
            await endpoint.Send(
                new SynchronizeRestaurantProductsResponse
                {
                    RestaurantId = context.Saga.RestaurantId,
                    ErrorMessage = error
                },
                r => r.RequestId = context.Saga.RequestId);
        }
    }

    public record SynchronizeRestaurantProductsRequest
    {
        public ulong RestaurantId { get; init; }
    }

    public record SynchronizeRestaurantProductsResponse
    {
        public ulong RestaurantId { get; init; }

        public string ErrorMessage { get; init; } = null!;
    }

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

    public class ProductSyncConsumer : IConsumer<SynchronizingData>
    {
        private readonly ProductsSyncer.ProductsSyncerClient _productsSyncer;

        private readonly IBus _bus;

        public ProductSyncConsumer(ProductsSyncer.ProductsSyncerClient productsSyncer, IBus bus)
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
}