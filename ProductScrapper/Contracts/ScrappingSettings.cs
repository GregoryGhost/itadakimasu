namespace ProductScrapper.Contracts;

public record ScrappingSettings
{
    public IReadOnlyList<string> ScrappingRestaurantUrls { get; init; } = Array.Empty<string>();

    public ProductsScrapperType ProductsScrapperType { get; init; } = ProductsScrapperType.MrTako;
}

public enum ProductsScrapperType
{
    MrTako = 0
}