namespace Itadakimasu.API.Gateway.Services;

using Itadakimasu.API.Gateway.DTOs.Pagination;

using JetBrains.Annotations;

[UsedImplicitly]
public class ProductsSynchronizationMapper
{
    public ProductsSynchronizatingPagination GetListRequest(PaginationSettings? pagination)
    {
        throw new NotImplementedException();
    }
}

public record ProductsSynchronizatingPagination
{
}