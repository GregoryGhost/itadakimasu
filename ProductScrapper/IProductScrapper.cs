namespace ProductScrapper;

// public interface IProductScrapper
// {
//     Result<IEnumerable<ScrappedProduct>, ScrappedError> ScrapProducts();
// }
//
// public record ScrappedProduct

public interface IProductScrapper
{
    Task<IEnumerable<ScrappedProduct>> ScrapProductsAsync();
}

public record ScrappedProduct
{
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
}