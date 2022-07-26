namespace Itadakimasu.ProductsAggregator.DAL;

using Itadakimasu.Core.DAL;

public class SynchronizatingRestaurant : BaseEntity
{
    public DateTime? EndSynchronization { get; set; }

    public Restaurant Restaurant { get; set; } = null!;
    
    public long RestaurantId { get; set; }

    public DateTime StartSynchronization { get; set; }

    public SynchronizationProductStatus Status { get; set; }

    public string ScrappingErrors { get; set; } = null!;
}