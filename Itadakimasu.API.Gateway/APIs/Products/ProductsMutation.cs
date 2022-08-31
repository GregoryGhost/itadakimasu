namespace Itadakimasu.API.Gateway.APIs.Products;

using HotChocolate.Subscriptions;

using Itadakimasu.API.Gateway.DTOs.Products;
using Itadakimasu.API.Gateway.Services;

using JetBrains.Annotations;

using Merchandiser.V1;

/// <summary>
///     API products mutation.
/// </summary>
[PublicAPI]
[ExtendObjectType(Name = "Mutation")]
public class ProductsMutation
{
    private readonly Merchandiser.MerchandiserClient _merchandiser;
    
    private readonly ITopicEventSender _sender;

    private readonly ProductsMapper _mapper;
    
    /// <summary>
    ///     Initialize dependencies.
    /// </summary>
    /// <param name="merchandiser">API products service.</param>
    public ProductsMutation(Merchandiser.MerchandiserClient merchandiser, ITopicEventSender sender, ProductsMapper mapper)
    {
        _merchandiser = merchandiser;
        _sender = sender;
        _mapper = mapper;
    }

    /// <summary>
    ///     Create a new product.
    /// </summary>
    /// <param name="dto">A new product info.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Returns created product.</returns>
    public async Task<CreatedProductDto> CreateProduct(CreatingProductDto dto, CancellationToken cancellationToken)
    {
        // var request = ToCreateProduct(dto);
        // var created = await _merchandiser.CreateProductAsync(request, cancellationToken: cancellationToken);
        // var createdProduct = ToCreatedProduct(created);
        //TODO: return code and remove ProductsSubscription.ProductAdded
        var createdProduct =  new CreatedProductDto
        {
            Id = 1,
            Name = dto.Name,
            Price = dto.Price
        };
        await _sender.SendAsync(nameof(ProductsSubscription.ProductAdded), createdProduct, cancellationToken);

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
        var request = _mapper.ToDeleteProduct(dto);
        var response = await _merchandiser.DeleteProductAsync(request, cancellationToken: cancellationToken);
        var deletedProduct = _mapper.ToDeletedProduct(dto);
    
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
        var request = _mapper.ToUpdateProduct(dto);
        var updated = await _merchandiser.UpdateProductAsync(request, cancellationToken: cancellationToken);
        var updatedProduct = _mapper.ToUpdatedProduct(updated);
    
        return updatedProduct;
    }
}