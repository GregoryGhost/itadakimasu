namespace Itadakimasu.API.Gateway.DTOs.Products;

public record ProductCardDto
{
    [GraphQLType(typeof(UnsignedLongType))]
    public ulong Id { get; init; }

    public bool IsNotFound { get; init; }

    public string Name { get; init; } = null!;

    public decimal Price { get; init; }
}