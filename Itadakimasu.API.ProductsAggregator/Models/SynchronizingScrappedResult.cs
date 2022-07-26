namespace Itadakimasu.API.ProductsAggregator.Models;

using ProductScrapper.Contracts;

public record SynchronizingScrappedResult
{
    public ScrappedResults ScrappedResults { get; init; } = null!;

    public long SynchronizingRequestId { get; init; }
}