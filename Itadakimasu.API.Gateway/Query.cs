namespace Itadakimasu.API.Gateway;

using Merchandiser.V1;

using PaginationOptions;

public class Query
{
    private readonly Merchandiser.MerchandiserClient _merchandiser;
    public Query(Merchandiser.MerchandiserClient merchandiser)
    {
        _merchandiser = merchandiser;
    }

    /// <summary>
    /// Get all available books.
    /// </summary>
    /// <returns>Returns all books.</returns>
    public async Task<IEnumerable<Product>> GetProducts()
    {
        var req = new ProductsPagination
        {
            Pagination = new Pagination
            {
                CurrentPage = 0,
                PageSize = 10,
                PageToken = string.Empty
            }
        };
        var list = await _merchandiser.ListProductsAsync(req);
        
        return list.Products.Select(x => new Product
        {
            Id = x.Id,
            Name = x.Name,
            Price = x.Price
        });
    }
}

public record Product 
{
    [GraphQLType(typeof(UnsignedLongType))]
    public ulong Id { get; init; }

    public string Name { get; init; } = null!;
    
    public decimal Price { get; init; }
}

