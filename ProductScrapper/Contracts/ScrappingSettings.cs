namespace ProductScrapper.Contracts;

public record ScrappingSettings
{
    public IReadOnlyList<string> ScrappingRestaurantUrls { get; init; } = Array.Empty<string>();
}