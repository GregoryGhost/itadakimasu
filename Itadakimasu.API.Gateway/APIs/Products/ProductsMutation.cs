namespace Itadakimasu.API.Gateway.APIs.Products;

using JetBrains.Annotations;

using Merchandiser.V1;

/// <summary>
///     API products mutation.
/// </summary>
[PublicAPI]
public class ProductsMutation
{
    private readonly Merchandiser.MerchandiserClient _merchandiser;

    /// <summary>
    ///     Initialize dependencies.
    /// </summary>
    /// <param name="merchandiser">API products service.</param>
    public ProductsMutation(Merchandiser.MerchandiserClient merchandiser)
    {
        _merchandiser = merchandiser;
    }

    /// <summary>
    ///     Create a new product.
    /// </summary>
    /// <param name="dto">A new product info.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Returns created product.</returns>
    public async Task<CreatedProductDto> CreateProduct(CreatingProductDto dto, CancellationToken cancellationToken)
    {
        var request = ToCreateProduct(dto);
        var created = await _merchandiser.CreateProductAsync(request, cancellationToken: cancellationToken);
        var createdProduct = ToCreatedProduct(created);

        return createdProduct;
    }

    /// <summary>
    ///     Delete a product.
    /// </summary>
    /// <param name="dto">Deleting product.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Returns deleted product.</returns>
    public async Task<DeletedProduct> DeleteProduct(DeletingProduct dto, CancellationToken cancellationToken)
    {
        var request = ToDeleteProduct(dto);
        var response = await _merchandiser.DeleteProductAsync(request, cancellationToken: cancellationToken);
        var deletedProduct = ToDeletedProduct(dto);

        return deletedProduct;
    }

    /// <summary>
    ///     Update a product.
    /// </summary>
    /// <param name="dto">Updating product.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Returns updated product.</returns>
    public async Task<UpdatedProduct> UpdateProduct(UpdatingProduct dto, CancellationToken cancellationToken)
    {
        var request = ToUpdateProduct(dto);
        var updated = await _merchandiser.UpdateProductAsync(request, cancellationToken: cancellationToken);
        var updatedProduct = ToUpdatedProduct(updated);

        return updatedProduct;
    }

    private static CreatedProductDto ToCreatedProduct(ProductDto created)
    {
        return new CreatedProductDto
        {
            Id = created.Id,
            Name = created.Name,
            Price = created.Price
        };
    }

    private static NewProductDto ToCreateProduct(CreatingProductDto dto)
    {
        return new NewProductDto
        {
            Name = dto.Name,
            Price = dto.Price
        };
    }

    private static DeletedProduct ToDeletedProduct(ProductInfoId dto)
    {
        return new DeletedProduct
        {
            Id = dto.Id
        };
    }

    private static ProductId ToDeleteProduct(ProductInfoId productInfoId)
    {
        return new ProductId
        {
            Id = productInfoId.Id
        };
    }

    private static UpdatedProduct ToUpdatedProduct(ProductDto updated)
    {
        return new UpdatedProduct
        {
            Id = updated.Id,
            Name = updated.Name,
            Price = updated.Price
        };
    }

    private static ProductDto ToUpdateProduct(UpdatingProduct dto)
    {
        return new ProductDto
        {
            Id = dto.Id,
            Name = dto.Name,
            Price = dto.Price
        };
    }
}

public record DeletingProduct : ProductInfoId
{
}

public record DeletedProduct : ProductInfoId
{
}

public record UpdatedProduct : UpdatingProduct
{
}

public record UpdatingProduct
{
    [GraphQLType(typeof(UnsignedLongType))]
    [ID]
    public ulong Id { get; init; }

    public string Name { get; init; } = null!;

    public decimal Price { get; init; }
}

public record CreatingProductDto
{
    public string Name { get; init; } = null!;

    public decimal Price { get; init; }
}

public record CreatedProductDto
{
    [GraphQLType(typeof(UnsignedLongType))]
    [ID]
    public ulong Id { get; init; }

    public string Name { get; init; } = null!;

    public decimal Price { get; init; }
}