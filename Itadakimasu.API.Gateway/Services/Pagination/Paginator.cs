namespace Itadakimasu.API.Gateway.Services.Pagination;

using PaginationOptions;

public class Paginator
{
    public  (bool HasNextPage, bool HasPrevPage) CheckPages(PageInfo pageInfo)
    {
        if (pageInfo.Page == 0)
            return (true, false);

        if (pageInfo.PageSize == pageInfo.TotalItems)
            return (false, false);

        var lastPage = pageInfo.TotalItems / pageInfo.PageSize;

        return pageInfo.Page == lastPage ? (false, true) : (true, true);
    }

    public void Test()
    {
        var (hasNextPage, hasPreviousPage) = _paginator.CheckPages(list.PageInfo);
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
}