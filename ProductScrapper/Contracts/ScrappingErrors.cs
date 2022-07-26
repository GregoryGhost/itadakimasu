namespace ProductScrapper.Contracts;

using Ardalis.SmartEnum;

public abstract class ScrappingErrors : SmartEnum<ScrappingErrors>
{
    public static readonly ScrappingErrors OnParsingMismatchProductNamesAndPrices = new MismatchProductNamesAndPricesError();

    public static readonly ScrappingErrors OnParsingNotFoundProductNames = new NotFoundProductNamesError();

    public static readonly ScrappingErrors OnParsingNotFoundProductPrices = new NotFoundProductPricesError();

    public static readonly ScrappingErrors ResponseError = new WebSiteResponseError();

    private ScrappingErrors(string name, int value)
        : base(name, value)
    {
    }

    private sealed class WebSiteResponseError : ScrappingErrors
    {
        public WebSiteResponseError()
            : base(nameof(WebSiteResponseError), 0)
        {
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "Error on making request to website.";
        }
    }

    private sealed class MismatchProductNamesAndPricesError : ScrappingErrors
    {
        public MismatchProductNamesAndPricesError()
            : base(nameof(MismatchProductNamesAndPricesError), 0)
        {
        }
        
        /// <inheritdoc />
        public override string ToString()
        {
            return "Error on match product names and prices.";
        }
    }

    private sealed class NotFoundProductNamesError : ScrappingErrors
    {
        public NotFoundProductNamesError()
            : base(nameof(NotFoundProductNamesError), 0)
        {
        }
        
        /// <inheritdoc />
        public override string ToString()
        {
            return "Error on parse product names.";
        }
    }

    private sealed class NotFoundProductPricesError : ScrappingErrors
    {
        public NotFoundProductPricesError()
            : base(nameof(NotFoundProductPricesError), 0)
        {
        }
        
        /// <inheritdoc />
        public override string ToString()
        {
            return "Error on parse product prices.";
        }
    }
}

public static class ScrappingErrorExtensions
{
    public static string GetScrappingErrors(this IEnumerable<ScrappingErrors> scrappingErrors)
    {
        var errors = scrappingErrors.Select(x => x.ToString());
        var formatted = string.Join("\n", errors);

        return formatted;
    }
}