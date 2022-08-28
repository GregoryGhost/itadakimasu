namespace Itadakimasu.API.ProductsSynchronizationSaga.PublicDTOs.DTOs;

using JetBrains.Annotations;

[PublicAPI]
public record SynchronizeRestaurantProductsResponse
{
    public ulong RestaurantId { get; init; }

    public string ErrorMessage { get; init; } = null!;
}