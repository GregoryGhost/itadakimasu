namespace Itadakimasu.API.Products.Services;

using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using Itadakimasu.Products.DAL;

using Merchandiser.V1;

using Microsoft.EntityFrameworkCore;

public class MerchandiserService : Merchandiser.MerchandiserBase
{
    private readonly AppDbContext _dbContext;

    private readonly ILogger<MerchandiserService> _logger;

    public MerchandiserService(ILogger<MerchandiserService> logger, AppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public override async Task<FoundProductDto> GetProduct(ProductId request, ServerCallContext context)
    {
        var foundProduct = await _dbContext.Products.SingleOrDefaultAsync(x => x.Id == request.Id);
        if (foundProduct is null)
            return new FoundProductDto
            {
                Null = NullValue.NullValue,
                Product = null
            };
        var mapped = MapDto(foundProduct);

        return mapped;
    }

    private static FoundProductDto MapDto(Product product)
    {
        var dto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price
        };

        return new FoundProductDto
        {
            Product = dto
        };
    }
}