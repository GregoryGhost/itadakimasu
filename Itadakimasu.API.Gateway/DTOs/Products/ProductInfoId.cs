namespace Itadakimasu.API.Gateway.DTOs.Products;

public record ProductInfoId
{
    [GraphQLType(typeof(UnsignedLongType))]
    [ID]
    public ulong Id { get; init; }
}