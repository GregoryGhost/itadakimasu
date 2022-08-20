namespace Itadakimasu.API.Gateway.APIs.ProductsSynchronization;

using HotChocolate.Types.Pagination;

using Itadakimasu.API.Gateway.DTOs.ProductsSynchronization;
using Itadakimasu.API.Gateway.Services.Pagination;

using JetBrains.Annotations;

using MassTransit;

[PublicAPI]
public class ProductsSynchronizationQuery
{
    private readonly IBus _bus;

    private readonly Paginator _paginator;

    public ProductsSynchronizationQuery(IBus bus, Paginator paginator)
    {
        _bus = bus;
        _paginator = paginator;
    }

    [UseOffsetPaging]
    public async Task<CollectionSegment<ProductsSynchronizationRequestDto>> GetProductsSynchronizationRequests(uint? skip, uint? take, CancellationToken cancellationToken)
    {
        var pagingInfo = null;
        var response = await _bus.Request<null, ProductsSynchronizationRequestDto>(pagingInfo);
        var paginated = _paginator.GetPaginated(response.Message);

        return paginated;
    }
}