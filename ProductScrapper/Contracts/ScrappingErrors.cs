namespace ProductScrapper.Contracts;

using Ardalis.SmartEnum;

public abstract class ScrappingErrors: SmartEnum<ScrappingErrors>
{
    public static readonly ScrappingErrors ResponseError = new WebSiteResponseError();
    public static readonly ScrappingErrors OnParsingMismatchProductNamesAndPrices = new MismatchProductNamesAndPricesError();
    public static readonly ScrappingErrors OnParsingNotFoundProductNames = new NotFoundProductNamesError();
    public static readonly ScrappingErrors OnParsingNotFoundProductPrices = new NotFoundProductPricesError();

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
    }
    
    private sealed class MismatchProductNamesAndPricesError : ScrappingErrors
    {
        public MismatchProductNamesAndPricesError()
            : base(nameof(MismatchProductNamesAndPricesError), 0)
        {
        }
    }
    
    private sealed class NotFoundProductNamesError : ScrappingErrors
    {
        public NotFoundProductNamesError()
            : base(nameof(NotFoundProductNamesError), 0)
        {
        }
    }
    
    private sealed class NotFoundProductPricesError : ScrappingErrors
    {
        public NotFoundProductPricesError()
            : base(nameof(NotFoundProductPricesError), 0)
        {
        }
    }
}