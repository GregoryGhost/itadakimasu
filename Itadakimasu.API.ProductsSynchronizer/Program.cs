using Grpc.Net.Client;

using Itadakimasu.API.ProductsSynchronizer.Services;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

var productsApiAddress = Environment.GetEnvironmentVariable("PRODUCTS_API_ADDRESS")
                       ?? builder.Configuration.GetSection("Api").GetSection("ProductsAddress").Value;

builder.Services.AddSingleton(
    s =>
    {
        var channel = GrpcChannel.ForAddress(productsApiAddress);

        var client = new Merchandiser.V1.Merchandiser.MerchandiserClient(channel);

        return client;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<ProductsSynchronizer>();
app.MapGet(
    "/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

var env = app.Environment;
if (env.IsDevelopment())
    app.MapGrpcReflectionService();

app.Run();