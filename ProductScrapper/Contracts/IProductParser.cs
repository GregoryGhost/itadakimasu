namespace ProductScrapper.Contracts;

using JetBrains.Annotations;

[PublicAPI]
public interface IProductParser<in TSourceData>
{
    Task<ScrappingResult> ParseProductsAsync(TSourceData sourceData);
}