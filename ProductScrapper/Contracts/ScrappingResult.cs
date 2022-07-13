namespace ProductScrapper.Contracts;

using CSharpFunctionalExtensions;

public record ScrappingResult
{
    public Result<IEnumerable<ScrappedProduct>, ScrappingErrors> Result { get; init; }

    public static implicit operator ScrappingResult(ScrappingErrors error)
    {
        return new ScrappingResult
        {
            Result = error
        };
    }

    public static implicit operator ScrappingResult(List<ScrappedProduct> value)
    {
        return new ScrappingResult
        {
            Result = value
        };
    }
}