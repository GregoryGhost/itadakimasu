namespace Itadakimasu.API.ProductsSynchronizationSaga.PublicDTOs.DTOs;

using JetBrains.Annotations;

[PublicAPI]
public record SynchronizeRestaurantProductsRequest
{
    public ulong RestaurantId { get; init; }
}