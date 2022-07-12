namespace Itadakimasu.Products.DAL;

using Itadakimasu.Core.DAL;

public class Restaurant : BaseEntity
{
    public string Name { get; set; } = null!;

    public List<Product> Products { get; set; } = null!;
}