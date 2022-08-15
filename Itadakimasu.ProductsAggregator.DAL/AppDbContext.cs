namespace Itadakimasu.ProductsAggregator.DAL;

using Microsoft.EntityFrameworkCore;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; } = null!;

    public DbSet<Restaurant> Restaurants { get; set; } = null!;

    public DbSet<SynchronizatingRestaurant> SynchronizatingRestaurants { get; set; } = null!;
}