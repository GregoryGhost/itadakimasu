namespace Itadakimasu.API.ProductsSynchronizationSaga.Types.Configs;

public record DependentMicroservicesAddresses
{
    public string ApiProductsAggregatorAddress { get; init; } = null!;
    public string ApiProductsSynchronizerAddress { get; init; } = null!;
}