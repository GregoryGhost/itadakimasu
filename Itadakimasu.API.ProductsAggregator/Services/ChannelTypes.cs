namespace Itadakimasu.API.ProductsAggregator.Services;

using System.Threading.Channels;

using Itadakimasu.API.ProductsAggregator.Models;
using Itadakimasu.ProductsAggregator.DAL;

public abstract class ProductsResultSynchronizationWriter : ChannelWriter<SynchronizingScrappedResult>
{
}

public abstract class ProductsResultSynchronizationReader : ChannelReader<SynchronizingScrappedResult>
{
}

public abstract class ProductsSynchronizationWriter : ChannelWriter<SynchronizatingRestaurant>
{
}

public abstract class ProductsSynchronizationReader : ChannelReader<SynchronizatingRestaurant>
{
}