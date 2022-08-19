namespace Itadakimasu.API.Gateway.APIs.Products;

using HotChocolate.Types.Pagination;

using Itadakimasu.API.Gateway.DTOs.Products;

using JetBrains.Annotations;

using Merchandiser.V1;

using PaginationOptions;

/// <summary>
///     API products queries.
/// </summary>
[PublicAPI]
public class ProductsQuery
{
    private const uint DEFAULT_PAGE_SIZE = 10;

    private const uint START_PAGE_INDEX = 0;

    private readonly Merchandiser.MerchandiserClient _merchandiser;

    /// <summary>
    ///     Initialize dependencies.
    /// </summary>
    /// <param name="merchandiser">API products service.</param>
    public ProductsQuery(Merchandiser.MerchandiserClient merchandiser)
    {
        _merchandiser = merchandiser;
    }

    /// <summary>
    ///     Get product by identifier.
    /// </summary>
    /// <param name="productInfoId">Product id.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Returns the requested product card.</returns>
    public async Task<ProductCardDto> GetProductById(ProductInfoId productInfoId, CancellationToken cancellationToken)
    {
        var request = ToProductId(productInfoId);
        var product = await _merchandiser.GetProductAsync(request, cancellationToken: cancellationToken);

        var productCard = GetProductCard(product);

        return productCard;
    }

    /// <summary>
    ///     Get products.
    /// </summary>
    /// <returns>Returns products.</returns>
    [UseOffsetPaging]
    public async Task<CollectionSegment<ProductInfoDto>> GetProducts(uint? skip, uint? take, CancellationToken cancellationToken)
    {
        var req = GetProductListRequest(skip, take);
        var list = await _merchandiser.ListProductsAsync(req, cancellationToken: cancellationToken);
        var collectionSegment = GetProductCollectionSegment(list);

        return collectionSegment;
    }

    private static (bool HasNextPage, bool HasPrevPage) CheckPages(PageInfo pageInfo)
    {
        if (pageInfo.Page == 0)
            return (true, false);

        if (pageInfo.PageSize == pageInfo.TotalItems)
            return (false, false);

        var lastPage = pageInfo.TotalItems / pageInfo.PageSize;

        return pageInfo.Page == lastPage ? (false, true) : (true, true);
    }

    private static ProductCardDto GetProductCard(FoundProductDto product)
    {
        var isNotFoundProduct = product.KindCase == FoundProductDto.KindOneofCase.Null;
        if (isNotFoundProduct)
            return new ProductCardDto
            {
                Id = 0,
                Name = string.Empty,
                Price = 0,
                IsNotFound = true
            };

        return new ProductCardDto
        {
            Id = product.Product.Id,
            Name = product.Product.Name,
            Price = product.Product.Price,
            IsNotFound = false
        };
    }

    private static CollectionSegment<ProductInfoDto> GetProductCollectionSegment(PaginatedProducts list)
    {
        var (hasNextPage, hasPreviousPage) = CheckPages(list.PageInfo);
        var pageInfo = new CollectionSegmentInfo(hasNextPage, hasPreviousPage);

        var products = list.Products.Select(
                               x => new ProductInfoDto
                               {
                                   Id = x.Id,
                                   Name = x.Name,
                                   Price = x.Price
                               })
                           .ToArray();
        var totalCount = ValueTask.FromResult((int) list.PageInfo.TotalItems);
        var collectionSegment = new CollectionSegment<ProductInfoDto>(
            products,
            pageInfo,
            _ => totalCount);

        return collectionSegment;
    }

    private static ProductsPagination GetProductListRequest(uint? skip, uint? take)
    {
        var currentPage = skip ?? START_PAGE_INDEX;
        var pageSize = take ?? DEFAULT_PAGE_SIZE;
        var req = new ProductsPagination
        {
            Pagination = new Pagination
            {
                CurrentPage = currentPage,
                PageSize = pageSize,
                PageToken = string.Empty
            }
        };

        return req;
    }

    private static ProductId ToProductId(ProductInfoId productInfoId)
    {
        return new ProductId
        {
            Id = productInfoId.Id
        };
    }
}