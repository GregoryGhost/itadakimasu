namespace ProductScrapper.Contracts;

using JetBrains.Annotations;

[PublicAPI]
public interface IProductHtmlParser : IProductParser<string>
{
}