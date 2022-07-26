namespace Itadakimasu.ProductsAggregator.DAL;

using Itadakimasu.Core.DAL;

public class RestaurantScrapper : BaseEntity
{
    public string ScrapperName { get; set; } = null!;
}