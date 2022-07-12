namespace Itadakimasu.API.Products.Services;

using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using Itadakimasu.Core.DAL;
using Itadakimasu.Products.DAL;

using Merchandiser.V1;

using Microsoft.EntityFrameworkCore;

/// <summary>
/// Merchandiser products service.
/// </summary>
public class MerchandiserService : Merchandiser.MerchandiserBase
{
    private readonly AppDbContext _dbContext;

    private readonly ILogger<MerchandiserService> _logger;

    /// <summary>
    /// Initialize depedencies.
    /// </summary>
    /// <param name="logger">Logger service.</param>
    /// <param name="dbContext">Database context.</param>
    public MerchandiserService(ILogger<MerchandiserService> logger, AppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public override async Task<ProductDto> CreateProduct(NewProductDto request, ServerCallContext context)
    {
        var isValidProduct = IsValidProduct(request, context);
        if (!isValidProduct)
            return null!;

        var newProduct = MapEntity(request);
        await _dbContext.Products.AddAsync(newProduct);
        await _dbContext.SaveChangesAsync();

        var createdProduct = MapDto(newProduct).Product;

        return createdProduct;
    }

    /// <inheritdoc />
    public override async Task<Empty> DeleteProduct(ProductId request, ServerCallContext context)
    {
        if (request.Id == 0)
        {
            context.Status = new Status(StatusCode.FailedPrecondition, "A product id must be above zero.");

            return null!;
        }

        var foundProduct = await _dbContext.Products.FindAsync(request.Id);
        if (foundProduct is null)
        {
            context.Status = new Status(StatusCode.NotFound, $"A product by id {request.Id} was not found.");

            return null!;
        }

        _dbContext.Products.Remove(foundProduct);

        return new Empty();
    }

    /// <inheritdoc />
    public override async Task<FoundProductDto> GetProduct(ProductId request, ServerCallContext context)
    {
        var foundProduct = await _dbContext.Products.SingleOrDefaultAsync(x => x.Id == (long)request.Id);
        if (foundProduct is null)
            return new FoundProductDto
            {
                Null = NullValue.NullValue,
                Product = null
            };
        var mapped = MapDto(foundProduct);

        return mapped;
    }

    /// <inheritdoc />
    public override async Task<PaginatedProducts> ListProducts(ProductsPagination request, ServerCallContext context)
    {
        var list = await _dbContext.Products
                                   .Paginate(request.Pagination)
                                   .Select(x => MapDto(x).Product)
                                   .ToListAsync();
        var pageInfo = await _dbContext.Products.ToPaginatedAsync(request.Pagination);
        var paginatedProducts = new PaginatedProducts
        {
            PageInfo = pageInfo,
            Products = {list}
        };

        return paginatedProducts;
    }

    /// <inheritdoc />
    public override async Task<ProductDto> UpdateProduct(ProductDto request, ServerCallContext context)
    {
        var isValidProduct = IsValidProduct(request, context);
        if (!isValidProduct)
            return null!;

        var foundProduct = await FindProductAsync(request, context);
        if (foundProduct is null)
            return null!;

        foundProduct.Name = request.Name;
        foundProduct.Price = request.Price;

        await _dbContext.SaveChangesAsync();

        var updated = MapDto(foundProduct).Product;

        return updated;
    }

    private async Task<Product?> FindProductAsync(ProductDto request, ServerCallContext context)
    {
        var foundProduct = await _dbContext.Products.FindAsync(request.Id);
        if (foundProduct is not null)
            return foundProduct;

        context.Status = new Status(StatusCode.NotFound, $"A product by id {request.Id} was not found.");

        return null;
    }

    private static bool IsValidProduct(NewProductDto request, ServerCallContext context)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            context.Status = new Status(StatusCode.FailedPrecondition, "Product name must have a name, but got an empty name.");

            return false;
        }

        if (request.Price <= 0)
        {
            context.Status = new Status(StatusCode.FailedPrecondition, "Product price must be above zero.");

            return false;
        }

        return true;
    }

    private static bool IsValidProduct(ProductDto request, ServerCallContext context)
    {
        var likeNewProduct = new NewProductDto
        {
            Name = request.Name,
            Price = request.Price
        };
        var isValid = IsValidProduct(likeNewProduct, context);

        return isValid;
    }

    private static FoundProductDto MapDto(Product product)
    {
        var dto = new ProductDto
        {
            Id = (ulong)product.Id,
            Name = product.Name,
            Price = product.Price
        };

        return new FoundProductDto
        {
            Product = dto
        };
    }

    private static Product MapEntity(NewProductDto newProductDto)
    {
        return new Product
        {
            Name = newProductDto.Name,
            Price = newProductDto.Price
        };
    }
}