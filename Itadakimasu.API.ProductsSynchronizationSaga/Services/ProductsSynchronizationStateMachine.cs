namespace Itadakimasu.API.ProductsSynchronizationSaga.Services
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using Google.Protobuf.WellKnownTypes;

    using Itadakimasu.API.ProductsSynchronizationSaga.PublicDTOs.DTOs;
    using Itadakimasu.API.ProductsSynchronizationSaga.Types.Saga;

    using JetBrains.Annotations;

    using MassTransit;
    

    using Microsoft.Extensions.Logging;

    using ProductsProxy.V1;

    using ProductsSyncer.V1;

    [UsedImplicitly]
    public sealed class ProductsSynchronizationStateMachine : MassTransitStateMachine<ProductsSynchronizationSagaState>
    {
        private readonly ILogger<ProductsSynchronizationStateMachine> _logger;
        
        public ProductsSynchronizationStateMachine(ILogger<ProductsSynchronizationStateMachine> logger)
        {
            _logger = logger;
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
            Request(() => SynchronizeRestaurantRequest);
            Request(() => CreateSynchronizationRequest);

            Initially(

                When(SynchronizeRestaurantProducts)
                    .Then(
                        x =>
                        {
                            if (!x.TryGetPayload(out SagaConsumeContext<ProductsSynchronizationSagaState, SynchronizeRestaurantProductsRequest>? payload))
                                throw new Exception("Unable to retrieve required payload for callback data.");

                            Debug.Assert(payload != null, nameof(payload) + " != null");
                            
                            x.Saga.RequestId = payload.RequestId;
                            x.Saga.ResponseAddress = payload.ResponseAddress;
                            x.Saga.RestaurantId = payload.Message.RestaurantId;
                        })
                    .Request(CreateSynchronizationRequest,
                        x =>
                        {
                            var data = new SynchronizationRestaurantRequest
                            {
                                RestaurantId = x.Saga.RestaurantId
                            };
                            
                            return x.Init<SynchronizationRestaurantRequest>(data);
                        })
                    .TransitionTo(CreateSynchronizationRequest.Pending)
            );
            
            During(
                CreateSynchronizationRequest.Pending,
                When(CreateSynchronizationRequest.Completed)
                    .Then(
                        x =>
                        {
                            x.Saga.SynchronizationRestaurantRequestId = x.Message.RequestId;
                        })
                    .TransitionTo(CreatedScrappingRestaurantProductsRequest)
                );

            During(
                CreatedScrappingRestaurantProductsRequest,
                When(ScrappedRestaurantProducts)
                    .Request(SynchronizeRestaurantRequest,
                        x =>
                        {
                            var data = new SynchronizingData
                            {
                                RequestId = x.Saga.SynchronizationRestaurantRequestId
                            };
                            
                            return x.Init<SynchronizingData>(data);
                        })
                    .TransitionTo(SynchronizingRestaurant)
            );

            During(
                SynchronizingRestaurant,
                When(GetSynchronizedRestaurant)
                    .TransitionTo(SynchronizedRestaurant)
                );

            During(
                SynchronizedRestaurant,
                When(SynchronizedRestaurant.Enter)
                    .ThenAsync(async context => { await RespondFromSagaAsync(context, string.Empty); })
                    .Finalize()
            );
        }

        public State CreatedScrappingRestaurantProductsRequest { get; init; } = null!;

        public State Failed { get; init; } = null!;

        public State SynchronizedRestaurant { get; init; } = null!;

        public State SynchronizingRestaurant { get; init; } = null!;

        public Event<SynchronizeRestaurantProductsRequest> SynchronizeRestaurantProducts { get; init; } = null!;

        public Event<SynchronizedRestaurantProductsRequest> ScrappedRestaurantProducts { get; init; } = null!;

        public Event<SynchronizedData> GetSynchronizedRestaurant { get; init; } = null!;

        public Request<ProductsSynchronizationSagaState, SynchronizingData, Empty> SynchronizeRestaurantRequest { get; init; } = null!;
        
        public Request<ProductsSynchronizationSagaState, SynchronizationRestaurantRequest,
            CreatedSynchronizationRestaurantRequest> CreateSynchronizationRequest { get; init; } = null!;

        private static async Task RespondFromSagaAsync(SagaConsumeContext<ProductsSynchronizationSagaState> context, string error)
        {
            if (context.Saga.ResponseAddress is null)
            {
                throw new Exception($"Provide {context.Saga.ResponseAddress} for send data to endpoint.");
            }
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
}