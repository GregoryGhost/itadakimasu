namespace Itadakimasu.Tests;

using FluentAssertions;

public class Tests
{
    private static readonly Product[] Products =
    {
        new()
        {
            Name = "Бони и Клайд (Для голодных ковбоев)",
            Price = 500
        },
        new()
        {
            Name = "Джокер",
            Price = 340
        },
        new()
        {
            Name = "Картофель фри",
            Price = 120
        },
        new()
        {
            Name = "Хьюстон (Бургер с беконом)",
            Price = 360
        },
        new()
        {
            Name = "Джанго (Острый бургер)",
            Price = 390
        },
        new()
        {
            Name = "Чикенбургер",
            Price = 350
        },
    };

    [TestCase("Product.jpg", 0)]
    [TestCase("UnknownProduct.png", null)]
    public void FuzzySearchFoodByImage(string productImageName, int? productIndex)
    {
        var imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, productImageName);
        var recognizedText = ProductDetector.FindProductByImage(imagePath, Products);

        var expected = GetExpectedProductByImage(productIndex);
        recognizedText.Should().BeEquivalentTo(expected);
    }

    private static FoundProductByImage? GetExpectedProductByImage(int? productIndex)
    {
        if (productIndex is null)
            return null;
        var expectedProduct = Products[productIndex.Value];
        var expected = new FoundProductByImage
        {
            ProductName = expectedProduct.Name,
            Price = expectedProduct.Price
        };

        return expected;
    }
}