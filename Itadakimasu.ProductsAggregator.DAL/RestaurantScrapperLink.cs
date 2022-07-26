namespace Itadakimasu.ProductsAggregator.DAL;

using Itadakimasu.Core.DAL;

public class RestaurantScrapperLink : BaseEntity
{
    public Restaurant Restaurant { get; set; } = null!;

    public RestaurantScrapper RestaurantScrapper { get; set; } = null!;
}