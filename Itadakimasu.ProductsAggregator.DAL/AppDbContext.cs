namespace Itadakimasu.ProductsAggregator.DAL;

using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; } = null!;

    public DbSet<Restaurant> Restaurants { get; set; } = null!;

    public DbSet<SynchronizatingProduct> SynchronizatingProducts { get; set; } = null!;
}