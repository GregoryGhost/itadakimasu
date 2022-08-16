namespace ProductScrapper.Services;

using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

using JetBrains.Annotations;

using ProductScrapper.Contracts;

[PublicAPI]
public class MrTakoParser : IProductHtmlParser
{
    private readonly HtmlParser _htmlParser;

    public MrTakoParser(HtmlParser htmlParser)
    {
        _htmlParser = htmlParser;
    }

    /// <inheritdoc />
    public async Task<ScrappingResult> ParseProductsAsync(string sourceData)
    {
        var document = await _htmlParser.ParseDocumentAsync(sourceData);
        var parsedProductNames = ParseProductNames(document);
        var parsedProductPrices = ParseProductPrices(document);
        var isEmptyProductNames = !parsedProductNames.Any();
        if (isEmptyProductNames)
            return ScrappingErrors.OnParsingNotFoundProductNames;

        var isEmptyProductPrices = !parsedProductPrices.Any();
        if (isEmptyProductPrices)
            return ScrappingErrors.OnParsingNotFoundProductPrices;

        var isNotEqualCountNamesAndPrices = parsedProductNames.Count != parsedProductPrices.Count;
        if (isNotEqualCountNamesAndPrices)
            return ScrappingErrors.OnParsingMismatchProductNamesAndPrices;

        var parsedProductInfos = MatchProductInfo(parsedProductNames, parsedProductPrices);

        return parsedProductInfos;
    }

    private static string GetSourcePrice(IElement element)
    {
        var productPrice = element.Attributes.FirstOrDefault(x => x.Name == "content")?.Value ?? string.Empty;

        return productPrice;
    }

    private static bool IsExistingProductPrice(IElement element)
    {
        var found = element.Attributes.FirstOrDefault(
            y => y.Name == "itemprop" && y.Value == "price");
        var isExistingPrice = found is not null;

        return isExistingPrice;
    }

    private static bool IsSoldOutProduct(IElement element)
    {
        var isSoldOutProduct = element.NodeName == "meta";
        var found = element.Attributes.FirstOrDefault(
            x => x.Name == "itemprop" && x.Value == "price");
        var isPrice = found is not null;
        var isCorrect = isSoldOutProduct && isPrice;

        return isCorrect;
    }

    private static List<ScrappedProduct> MatchProductInfo(List<string> parsedProductNames, List<string> parsedProductPrices)
    {
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
                                                   .Select(
                                                       x => new ScrappedProduct
                                                       {
                                                           Name = x.ProductName,
                                                           Price = x.ProductPrice ??
                                                                   throw new Exception("Product price have no value.")
                                                       })
                                                   .ToList();
        return parsedProductInfos;
    }

    private static List<string> ParseProductNames(IHtmlDocument document)
    {
        var parsedProductNames = document.QuerySelectorAll(".cat-item-content-T")
                                         .Select(x => x.InnerHtml)
                                         .ToList();
        return parsedProductNames;
    }

    private static List<string> ParseProductPrices(IHtmlDocument document)
    {
        var parsedProductPrices = document.QuerySelectorAll("span, meta")
                                          .Where(x => IsExistingProductPrice(x) || IsSoldOutProduct(x))
                                          .Select(
                                              x =>
                                              {
                                                  if (IsExistingProductPrice(x))
                                                      return x.InnerHtml;

                                                  if (IsSoldOutProduct(x))
                                                      return GetSourcePrice(x);

                                                  return string.Empty;
                                              })
                                          .ToList();
        return parsedProductPrices;
    }
}

[PublicAPI]
public class MrTakoScrapper : ProductWebSiteScrapper
{
    // ReSharper disable once SuggestBaseTypeForParameterInConstructor
    public MrTakoScrapper(MrTakoClient httpClient, ScrappingSettings scrappingSettings, MrTakoParser productParser)
        : base(scrappingSettings, productParser, httpClient)
    {
    }
}