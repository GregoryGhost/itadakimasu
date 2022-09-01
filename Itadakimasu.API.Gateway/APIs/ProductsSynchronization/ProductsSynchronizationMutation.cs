namespace Itadakimasu.API.Gateway.APIs.ProductsSynchronization;

using Itadakimasu.API.Gateway.DTOs.ProductsSynchronization;
using Itadakimasu.API.ProductsSynchronizationSaga.PublicDTOs.DTOs;

using JetBrains.Annotations;

using MassTransit;

[PublicAPI]
[ExtendObjectType(Name = "Mutation")]
public class ProductsSynchronizationMutation
{
    public async Task<bool> CreateProductsSynchronizationRequest(CreatingSynchronizationRequestDto newRequestDto, [Service] IPublishEndpoint publishEndpoint, CancellationToken cancellationToken)
    {
        var synchronizationRequest = new SynchronizeRestaurantProductsRequest
        {
            RestaurantId = newRequestDto.RestaurantId
        };
        await publishEndpoint.Publish(synchronizationRequest, cancellationToken);

        return true;
    }

    public async Task<bool> CancelProductsSynchronizationRequest(CancelProductsSynchronizationRequestDto cancelProductsSynchronizationRequestDto, [Service] IPublishEndpoint publishEndpoint, CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(cancelProductsSynchronizationRequestDto, cancellationToken);

        return true;
    }
}