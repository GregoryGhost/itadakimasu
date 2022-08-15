namespace Itadakimasu.API.ProductsAggregator.Services;

using System.Threading.Channels;

using Itadakimasu.API.ProductsAggregator.Models;
using Itadakimasu.ProductsAggregator.DAL;

public sealed class ProductsResultSynchronizationWriter
{
    public ChannelWriter<SynchronizingScrappedResult> ChannelWriter { get; init; } = null!;
}

public sealed class ProductsResultSynchronizationReader
{
    public ChannelReader<SynchronizingScrappedResult> ChannelReader { get; init; } = null!;
}

public sealed class ProductsSynchronizationWriter
{
    public ChannelWriter<SynchronizatingRestaurant> ChannelWriter { get; init; } = null!;
}

public sealed class ProductsSynchronizationReader
{
    public ChannelReader<SynchronizatingRestaurant> ChannelReader { get; init; } = null!;
}