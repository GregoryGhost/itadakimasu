namespace Itadakimasu.API.Gateway.Services.Pagination;

using HotChocolate.Types.Pagination;

using JetBrains.Annotations;

using PaginationOptions;

[UsedImplicitly]
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

    public CollectionSegment<TData> GetPaginatedData<TData>(PaginatedData<TData> data)
    {
        var (hasNextPage, hasPreviousPage) = CheckPages(data.PageInfo);
        var pageInfo = new CollectionSegmentInfo(hasNextPage, hasPreviousPage);
        
        var totalCount = ValueTask.FromResult((int) data.PageInfo.TotalItems);
        var collectionSegment = new CollectionSegment<TData>(
            data.Items,
            pageInfo,
            _ => totalCount);

        return collectionSegment;
    }
}

[UsedImplicitly]
public record PaginatedData<T>
{
    public IReadOnlyCollection<T> Items { get; init; } = null!;

    public PageInfo PageInfo { get; init; } = null!;
}