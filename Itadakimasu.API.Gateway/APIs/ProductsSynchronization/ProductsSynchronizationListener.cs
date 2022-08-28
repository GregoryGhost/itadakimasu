namespace Itadakimasu.API.Gateway.APIs.ProductsSynchronization;

using HotChocolate.Subscriptions;

using Itadakimasu.API.Gateway.DTOs.ProductsSynchronization;
using Itadakimasu.API.ProductsSynchronizationSaga.PublicDTOs.DTOs;

using JetBrains.Annotations;

using MassTransit;

[UsedImplicitly]
public class ProductsSynchronizationListener: IConsumer<SynchronizeRestaurantProductsResponse>
{
    private readonly ITopicEventSender _sender;

    public ProductsSynchronizationListener(ITopicEventSender sender)
    {
        _sender = sender;
    }

    public async Task Consume(ConsumeContext<SynchronizeRestaurantProductsResponse> context)
    {
        var message = new SynchronizedProductsRequestDto
        {
            RestaurantId = context.Message.RestaurantId
        };
        await _sender.SendAsync(nameof(ProductsSycnhronizationSubscription.SynchronizedProductsRequest), message);
    }
}