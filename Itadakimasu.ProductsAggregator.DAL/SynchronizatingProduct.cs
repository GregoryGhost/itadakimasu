namespace Itadakimasu.ProductsAggregator.DAL;

using Itadakimasu.Core.DAL;

public class SynchronizatingProduct: BaseEntity
{
    public SynchronizationProductStatus Status { get; set; }

    public Restaurant Restaurant { get; set; } = null!;
    
    public DateTime StartSynchronization { get; set; }
    
    public DateTime? EndSynchronization { get; set; }
}