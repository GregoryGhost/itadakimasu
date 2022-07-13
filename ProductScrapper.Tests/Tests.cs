namespace ProductScrapper.Tests;

using FluentAssertions;

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
        var actual = (await inputData.Parser.ParseProductsAsync(inputData.SourceParsingData))
            .ToList();

        actual.Should().BeEquivalentTo(expected.ScrappedProducts);
    }

    [TestCaseSource(typeof(MrTakoScrapTestDataTestCases))]
    public async Task TestScrapProductsAsync(string testCaseName, ScrappingInputData inputData, ExpectedProducts expected)
    {
        var actual = (await inputData.Scrapper.ScrapProductsAsync()).ToList();

        actual.Should().BeEquivalentTo(expected.ScrappedProducts);
    }

    [TestCaseSource(typeof(MrTakoScrapRealWebsiteTestCases))]
    public async Task TestScrapRealProductsAsync(string testCaseName, ScrappingInputData inputData, ExpectedProducts _)
    {
        var scrappedProducts = (await inputData.Scrapper.ScrapProductsAsync()).ToList();

        scrappedProducts.Should().NotBeEmpty();

        var formatted = string.Join(", ", scrappedProducts);
        Console.WriteLine($"Total count: {scrappedProducts.Count}. Scrapped products: {formatted}");

        Assert.Pass();
    }
}