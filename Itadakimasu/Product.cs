namespace Itadakimasu;

public record Product
{
    public string Name { get; init; } = null!;
    public decimal Price { get; init; }
}