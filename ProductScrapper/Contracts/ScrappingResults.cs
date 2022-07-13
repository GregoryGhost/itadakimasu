namespace ProductScrapper.Contracts;

public record ScrappingResults
{
    public IEnumerable<ScrappingErrors> Errors { get; init; } = Enumerable.Empty<ScrappingErrors>();

    public IEnumerable<ScrappedProduct> ScrappedProducts { get; init; } = Enumerable.Empty<ScrappedProduct>();
}