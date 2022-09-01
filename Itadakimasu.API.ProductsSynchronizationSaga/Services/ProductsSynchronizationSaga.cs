namespace Itadakimasu.API.ProductsSynchronizationSaga.Services;

using Itadakimasu.API.ProductsSynchronizationSaga.PublicDTOs.DTOs;

using JetBrains.Annotations;

using MassTransit;

[UsedImplicitly]
public class ProductsSynchronizationSaga: ISaga, InitiatedByOrOrchestrates<SynchronizeRestaurantProductsRequest>
{
    public Guid CorrelationId { get; set; }

    public Task Consume(ConsumeContext<SynchronizeRestaurantProductsRequest> context)
    {
        CorrelationId = context.Message.CorrelationId;
        
        return Task.CompletedTask;
    }
}