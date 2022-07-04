namespace Itadakimasu.Products.Tests;

using Google.Protobuf.WellKnownTypes;

using Itadakimasu.API.Products.Services;
using Itadakimasu.Products.DAL;

using Merchandiser.V1;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;

public class Tests
{
    private readonly AppDbContext _inMemoryDb;

    private readonly MerchandiserService _merchandiserService;

    public Tests()
    {
        var mockLogger = new Mock<ILogger<MerchandiserService>>();
        var options = new DbContextOptionsBuilder<AppDbContext>()
                      .UseInMemoryDatabase("Test")
                      .Options;
        _inMemoryDb = new AppDbContext(options);
        _merchandiserService = new MerchandiserService(mockLogger.Object, _inMemoryDb);
    }

    [SetUp]
    public void Setup()
    {
        _inMemoryDb.Database.EnsureDeleted();
    }

    [Test]
    public async Task TestGetProductByIdAsync()
    {
        //Arrange
        var msg = new ProductId
        {
            Id = 1
        };

        var actualProduct = new Product
        {
            Name = "kekw",
            Price = 322
        };

        _inMemoryDb.Products.Add(actualProduct);
        await _inMemoryDb.SaveChangesAsync();

        //Act
        var actual = await _merchandiserService.GetProduct(msg, TestServerCallContext.Create());

        //Assert
        var expected = new FoundProductDto
        {
            Product = new ProductDto
            {
                Id = actualProduct.Id,
                Name = actualProduct.Name,
                Price = actualProduct.Price
            }
        };

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public async Task TestIntegrationAsync()
    {
        //Arrange
        var msg = new ProductId
        {
            Id = 1
        };

        //Act
        var actual = await _merchandiserService.GetProduct(msg, TestServerCallContext.Create());

        //Assert
        var expected = new FoundProductDto
        {
            Null = NullValue.NullValue,
            Product = null
        };

        Assert.That(actual, Is.EqualTo(expected));
    }
}