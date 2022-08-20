namespace Itadakimasu.API.Gateway.DTOs.ProductsSynchronization;

using JetBrains.Annotations;

[PublicAPI]
public record CreatingSynchronizationRequestDto
{
    [GraphQLType(typeof(UnsignedLongType))]
    [ID]
    public ulong RestaurantId { get; init; }
}