namespace ProductScrapper.Tests;

using FluentAssertions;
using FluentAssertions.CSharpFunctionalExtensions;

using ProductScrapper.Tests.TestCases;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [TestCaseSource(typeof(MrTakoParseTestDataTestCases))]
    public async Task TestParseProductsAsync(string testCaseName, ParsingInputData inputData, ExpectedProducts expected)
    {
        var actual = await inputData.Parser.ParseProductsAsync(inputData.SourceParsingData);

        actual.Result.Should().BeSuccess();
        actual.Result.Value.Should().BeEquivalentTo(expected.ScrappedProducts);
    }

    [TestCaseSource(typeof(MrTakoScrapTestDataTestCases))]
    public async Task TestScrapProductsAsync(string testCaseName, ScrappingInputData inputData, ExpectedProducts expected)
    {
        var actual = await inputData.Scrapper.ScrapProductsAsync();

        actual.Errors.Should().BeEmpty();
        actual.ScrappedProducts.ToList().Should().BeEquivalentTo(expected.ScrappedProducts);
    }

    [TestCaseSource(typeof(MrTakoScrapRealWebsiteTestCases))]
    public async Task TestScrapRealProductsAsync(string testCaseName, ScrappingInputData inputData, ExpectedProducts _)
    {
        var actual = await inputData.Scrapper.ScrapProductsAsync();

        actual.Errors.Should().BeEmpty();

        var scrappedProducts = actual.ScrappedProducts.ToList();
        scrappedProducts.Should().NotBeEmpty();

        var formatted = string.Join(", ", scrappedProducts);
        Console.WriteLine($"Total count: {scrappedProducts.Count}. Scrapped products: {formatted}");

        Assert.Pass();
    }
}