namespace Itadakimasu;

public record FoundProductByImage
{
    public decimal Price { get; init; }

    public string ProductName { get; init; } = null!;
}