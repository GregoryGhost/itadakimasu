namespace Itadakimasu.API.Gateway.Services;

using HotChocolate.Types.Pagination;

using Itadakimasu.API.Gateway.DTOs.Pagination;
using Itadakimasu.API.Gateway.DTOs.Products;
using Itadakimasu.API.Gateway.Services.Pagination;

using JetBrains.Annotations;

using Merchandiser.V1;

[UsedImplicitly]
public class ProductsMapper
{
    private const uint DEFAULT_PAGE_SIZE = 10;

    private const uint START_PAGE_INDEX = 0;

    private readonly Paginator _paginator;

    public ProductsMapper(Paginator paginator)
    {
        _paginator = paginator;
    }

    public ProductCardDto GetProductCard(FoundProductDto product)
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

    public ProductsPagination GetProductListRequest(PaginationSettings? pagination)
    {
        var currentPage = pagination?.Skip ?? START_PAGE_INDEX;
        var pageSize = pagination?.Take ?? DEFAULT_PAGE_SIZE;
        var req = new ProductsPagination
        {
            Pagination = new PaginationOptions.Pagination
            {
                CurrentPage = currentPage,
                PageSize = pageSize,
                PageToken = string.Empty
            }
        };

        return req;
    }

    public ProductId ToProductId(ProductInfoId productInfoId)
    {
        return new ProductId
        {
            Id = productInfoId.Id
        };
    }
    
    public CollectionSegment<ProductInfoDto> GetProductCollectionSegment(PaginatedProducts list)
    {
        var products = list.Products.Select(
                               x => new ProductInfoDto
                               {
                                   Id = x.Id,
                                   Name = x.Name,
                                   Price = x.Price
                               })
                           .ToArray();
        var data = new PaginatedData<ProductInfoDto>
        {
            Items = products,
            PageInfo = list.PageInfo
        };
        var paged = _paginator.GetPaginatedData(data);

        return paged;
    }

    private CreatedProductDto ToCreatedProduct(ProductDto created)
    {
        return new CreatedProductDto
        {
            Id = created.Id,
            Name = created.Name,
            Price = created.Price
        };
    }

    private NewProductDto ToCreateProduct(CreatingProductDto dto)
    {
        return new NewProductDto
        {
            Name = dto.Name,
            Price = dto.Price
        };
    }

    public DeletedProduct ToDeletedProduct(ProductInfoId dto)
    {
        return new DeletedProduct
        {
            Id = dto.Id
        };
    }

    public ProductId ToDeleteProduct(ProductInfoId productInfoId)
    {
        return new ProductId
        {
            Id = productInfoId.Id
        };
    }

    public UpdatedProduct ToUpdatedProduct(ProductDto updated)
    {
        return new UpdatedProduct
        {
            Id = updated.Id,
            Name = updated.Name,
            Price = updated.Price
        };
    }

    public ProductDto ToUpdateProduct(UpdatingProduct dto)
    {
        return new ProductDto
        {
            Id = dto.Id,
            Name = dto.Name,
            Price = dto.Price
        };
    }
}