namespace Itadakimasu.API.Gateway.APIs.Products;

using HotChocolate.Types.Pagination;

using Itadakimasu.API.Gateway.DTOs.Pagination;
using Itadakimasu.API.Gateway.DTOs.Products;
using Itadakimasu.API.Gateway.Services;
using Itadakimasu.API.Gateway.Services.Pagination;

using JetBrains.Annotations;

using Merchandiser.V1;

using PaginationOptions;

/// <summary>
///     API products queries.
/// </summary>
[PublicAPI]
[ExtendObjectType(Name = "Query")]
public class ProductsQuery
{
    private readonly Merchandiser.MerchandiserClient _merchandiser;

    private readonly ProductsMapper _mapper;

    /// <summary>
    ///     Initialize dependencies.
    /// </summary>
    /// <param name="merchandiser">API products service.</param>
    public ProductsQuery(Merchandiser.MerchandiserClient merchandiser, ProductsMapper mapper)
    {
        _merchandiser = merchandiser;
        _mapper = mapper;
    }

    /// <summary>
    ///     Get product by identifier.
    /// </summary>
    /// <param name="productInfoId">Product id.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Returns the requested product card.</returns>
    public async Task<ProductCardDto> GetProductById(ProductInfoId productInfoId, CancellationToken cancellationToken)
    {
        var request = _mapper.ToProductId(productInfoId);
        var product = await _merchandiser.GetProductAsync(request, cancellationToken: cancellationToken);
        var productCard = _mapper.GetProductCard(product);

        return productCard;
    }

    /// <summary>
    ///     Get products.
    /// </summary>
    /// <returns>Returns products.</returns>
    [UseOffsetPaging]
    public async Task<CollectionSegment<ProductInfoDto>> GetProducts(PaginationSettings? pagination, CancellationToken cancellationToken)
    {
        var req = _mapper.GetProductListRequest(pagination);
        var list = await _merchandiser.ListProductsAsync(req, cancellationToken: cancellationToken);
        var collectionSegment = _mapper.GetProductCollectionSegment(list);

        return collectionSegment;
    }
}