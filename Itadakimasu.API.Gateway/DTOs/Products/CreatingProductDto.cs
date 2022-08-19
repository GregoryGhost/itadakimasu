namespace Itadakimasu.API.Gateway.DTOs.Products;

public record CreatingProductDto
{
    public string Name { get; init; } = null!;

    public decimal Price { get; init; }
}