namespace Itadakimasu.API.Gateway.DTOs.Products;

public record UpdatingProduct
{
    [GraphQLType(typeof(UnsignedLongType))]
    [ID]
    public ulong Id { get; init; }

    public string Name { get; init; } = null!;

    public decimal Price { get; init; }
}