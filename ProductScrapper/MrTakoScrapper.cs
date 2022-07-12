namespace ProductScrapper;

using AngleSharp.Html.Parser;

using JetBrains.Annotations;

[PublicAPI]
public class MrTakoParser : IProductHtmlParser
{
    private readonly HtmlParser _htmlParser;

    public MrTakoParser(HtmlParser htmlParser)
    {
        _htmlParser = htmlParser;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ScrappedProduct>> ParseProductsAsync(string sourceData)
    {
        var document = await _htmlParser.ParseDocumentAsync(sourceData);
        var parsedProductNames = document.QuerySelectorAll(".cat-item-content-T")
                                         .Select(x => x.InnerHtml)
                                         .ToList();
        var parsedProductPrices = document.QuerySelectorAll("span")
                                          .Where(x => x.Attributes.FirstOrDefault(y => y.Name == "itemprop" && y.Value == "price") is not null)
                                          .Select(x => x.InnerHtml)
                                          .ToList();
        var isNotEqualCountNamesAndPrices = parsedProductNames.Count != parsedProductPrices.Count;
        if (isNotEqualCountNamesAndPrices)
        {
            //TODO: replace on error result
            return Enumerable.Empty<ScrappedProduct>();
        }

        var isEmptyProductNames = !parsedProductNames.Any();
        if (isEmptyProductNames)
        {
            //TODO: replace on error result
            return Enumerable.Empty<ScrappedProduct>();
        }
        
        var isEmptyProductPrices = !parsedProductPrices.Any();
        if (isEmptyProductPrices)
        {
            //TODO: replace on error result
            return Enumerable.Empty<ScrappedProduct>();
        }

        var parsedProductInfos = parsedProductNames.Zip(parsedProductPrices)
                                                   .Select(
                                                       sourceProductInfo =>
                                                       {
                                                           var sourceProductName = sourceProductInfo.First;
                                                           var sourceProductPrice = sourceProductInfo.Second;

                                                           var parsedProductPrice = decimal.TryParse(
                                                               sourceProductPrice,
                                                               out var price)
                                                               ? price
                                                               : (decimal?) null;

                                                           return (ProductName: sourceProductName,
                                                               ProductPrice: parsedProductPrice);
                                                       })
                                                   .Where(
                                                       x => !string.IsNullOrWhiteSpace(x.ProductName) &&
                                                            x.ProductPrice is not null)
                                                   .Select(x => new ScrappedProduct
                                                   {
                                                       Name = x.ProductName,
                                                       Price = x.ProductPrice ?? throw new Exception("Product price have no value.")
                                                   })
                                                   .ToList();

        return parsedProductInfos;
    }
}

[PublicAPI]
public class MrTakoScrapper: ProductWebSiteScrapper
{
    // ReSharper disable once SuggestBaseTypeForParameterInConstructor
    public MrTakoScrapper(HttpClient httpClient, ScrappingSettings scrappingSettings, MrTakoParser productParser)
        : base(scrappingSettings, productParser, httpClient)
    {
    }
}

public record ScrappingSettings
{
    public IReadOnlyList<string> ScrappingRestaurantUrls { get; init; } = Array.Empty<string>();
}