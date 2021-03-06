namespace Itadakimasu.Order.Tests;

using Itadakimasu.API.Order.Services;

using Microsoft.Extensions.Logging;

using Moq;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task TestIntegrationAsync()
    {
        //Arrange
        var mockLogger = new Mock<ILogger<GreeterService>>();
        var greeterService = new GreeterService(mockLogger.Object);

        var msg = new HelloRequest
        {
            Name = "testName"
        };

        //Act
        var actual = await greeterService.SayHello(msg, TestServerCallContext.Create());

        //Assert
        var expected = new HelloReply
        {
            Message = "Hello " + msg.Name
        };

        Assert.That(actual, Is.EqualTo(expected));
    }
}