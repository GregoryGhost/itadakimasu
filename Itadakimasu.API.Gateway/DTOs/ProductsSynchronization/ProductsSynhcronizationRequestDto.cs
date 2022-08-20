namespace Itadakimasu.API.Gateway.DTOs.ProductsSynchronization;

using JetBrains.Annotations;

[PublicAPI]
public record ProductsSynhcronizationRequestDto
{
    [GraphQLType(typeof(UnsignedLongType))]
    [ID]
    public ulong RestaurantId { get; init; }
}