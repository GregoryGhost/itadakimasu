namespace Itadakimasu.API.Products.Services;

using Greet.V1;

using Grpc.Core;

public class GreeterService : Greeter.GreeterBase
{
    private readonly ILogger<GreeterService> _logger;

    public GreeterService(ILogger<GreeterService> logger)
    {
        _logger = logger;
    }

    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        return Task.FromResult(
            new HelloReply
            {
                Message = "Hello " + request.Name
            });
    }
}