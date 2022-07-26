namespace Itadakimasu.ProductsAggregator.DAL;

using Itadakimasu.Core.DAL;

public class Restaurant : BaseEntity
{
    public string Name { get; set; } = null!;

    public List<Product> Products { get; set; } = null!;

    public List<SynchronizatingRestaurant> SynchronizatingRestaurants { get; set; } = null!;

    public RestaurantScrapper RestaurantScrapper { get; set; } = null!;
}