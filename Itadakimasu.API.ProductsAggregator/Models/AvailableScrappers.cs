namespace Itadakimasu.API.ProductsAggregator.Models;

using ProductScrapper.Contracts;

public record AvailableScrappers
{
    public IReadOnlyCollection<IProductWebSiteScrapper> Scrappers { get; init; } = null!;
}