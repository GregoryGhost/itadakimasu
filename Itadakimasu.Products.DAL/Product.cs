namespace Itadakimasu.Products.DAL;

using Itadakimasu.Core.DAL;

public class Product : BaseEntity
{
    public string Name { get; set; } = null!;

    public decimal Price { get; set; }
}