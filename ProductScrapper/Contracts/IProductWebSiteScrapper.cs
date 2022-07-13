namespace ProductScrapper.Contracts;

using JetBrains.Annotations;

[PublicAPI]
public interface IProductWebSiteScrapper : IProductScrapper<string>
{
}