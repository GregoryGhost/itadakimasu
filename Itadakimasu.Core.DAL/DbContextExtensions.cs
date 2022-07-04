namespace Itadakimasu.Core.DAL;

using Microsoft.EntityFrameworkCore;

using PaginationOptions;

public static class DbContextExtensions
{
    public static IQueryable<TEntity> Paginate<TEntity>(this IQueryable<TEntity> query, Pagination pagination)
    {
        var currentPage = (int) pagination.CurrentPage;
        var pageSize = (int) pagination.PageSize;
        var paginated = query.Skip(currentPage)
                             .Take(pageSize);

        return paginated;
    }

    public static async Task<PageInfo> ToPaginatedAsync<TEntity>(this DbSet<TEntity> dbSet, Pagination pagination)
        where TEntity : class
    {
        var totalItemsLength = (uint) await dbSet.CountAsync();
        var pageInfo = new PageInfo
        {
            Page = pagination.CurrentPage,
            PageSize = pagination.PageSize,
            TotalItems = totalItemsLength
        };

        return pageInfo;
    }
}