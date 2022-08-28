namespace Itadakimasu.API.Gateway.APIs.ProductsSynchronization;

using HotChocolate.Types.Pagination;

using Itadakimasu.API.Gateway.DTOs.Pagination;
using Itadakimasu.API.Gateway.DTOs.ProductsSynchronization;
using Itadakimasu.API.Gateway.Services;
using Itadakimasu.API.Gateway.Services.Pagination;

using JetBrains.Annotations;

using MassTransit;

[PublicAPI]
public class ProductsSynchronizationQuery
{
    private readonly IBus _bus;

    private readonly ProductsSynchronizationMapper _mapper;

    private readonly Paginator _paginator;

    public ProductsSynchronizationQuery(IBus bus, Paginator paginator, ProductsSynchronizationMapper mapper)
    {
        _bus = bus;
        _paginator = paginator;
        _mapper = mapper;
    }

    [UseOffsetPaging]
    public async Task<CollectionSegment<ProductsSynchronizationRequestDto>> GetProductsSynchronizationRequests(
        PaginationSettings? pagination, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
        // var pagingInfo = _mapper.GetListRequest(pagination);
        // //TODO: make typed request client for products synchronization over the bus
        // var response = await _bus.Request < null, ProductsSynchronizationRequestDto>(pagingInfo);
        // var paginated = _paginator.GetPaginatedData(response.Message);
        //
        // return paginated;
    }
}
