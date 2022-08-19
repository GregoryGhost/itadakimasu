namespace Itadakimasu.API.Gateway.DTOs.Products;

public record ProductInfoId
{
    [GraphQLType(typeof(UnsignedLongType))]
    public ulong Id { get; init; }
}