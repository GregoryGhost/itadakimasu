namespace Itadakimasu;

public record FoundProductByImage
{
    public string ProductName { get; init; } = null!;
    public decimal Price { get; init; }
}