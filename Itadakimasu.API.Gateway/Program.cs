using Itadakimasu.API.Gateway.APIs.Products;
using Itadakimasu.API.Gateway.APIs.ProductsSynchronization;
using Itadakimasu.API.Gateway.Services;
using Itadakimasu.API.Gateway.Services.Pagination;
using Itadakimasu.API.Gateway.Types.Configs;

using MassTransit;

var builder = WebApplication.CreateBuilder(args);

RegisterGraphQlTypes(builder);
RegisterGrpcClients(builder);
RegisterMassTransit(builder);

RegisterStaffServices(builder);

var app = builder.Build();

app.UseWebSockets();
app.MapGraphQL();

app.Run();

static void RegisterStaffServices(WebApplicationBuilder builder)
{
    builder.Services.AddSingleton<Paginator>();
    builder.Services.AddSingleton<ProductsMapper>();
}

static void RegisterGraphQlTypes(WebApplicationBuilder builder)
{
    builder.Services
           .AddGraphQLServer()
           .AddInMemorySubscriptions()
           .AddQueryType<ProductsQuery>()
           .AddMutationType<ProductsMutation>()
           .AddSubscriptionType<ProductsSubscription>()
           .AddMutationType<ProductsSynchronizationMutation>()
           .AddQueryType<ProductsSynchronizationQuery>()
           .AddSubscriptionType<ProductsSycnhronizationSubscription>();
}

static void RegisterGrpcClients(WebApplicationBuilder builder)
{
    builder.Services.AddGrpcClient<Merchandiser.V1.Merchandiser.MerchandiserClient>(
                options =>
                {
                    var merchandiserAddress = Environment.GetEnvironmentVariable("API_PRODUCTS_ADDRESS");
                    if (string.IsNullOrWhiteSpace(merchandiserAddress))
                        throw new Exception("You must provide correct merchandiser address.");

                    options.Address = new Uri(merchandiserAddress);
                })
            .ConfigureChannel(
                options => { options.UnsafeUseInsecureChannelCallCredentials = true; });
}

static void RegisterMassTransit(WebApplicationBuilder builder)
{
    var rabbitMqConfig = ConfigHelper.GetRabbitMqConfig();
    builder.Services.AddMassTransit(x =>
    {
        x.UsingRabbitMq(
            (context, cfg) =>
            {
                cfg.Host(rabbitMqConfig.Address, "/", h =>
                {
                    h.Username(rabbitMqConfig.Login);
                    h.Password(rabbitMqConfig.Password);
                });

                cfg.UseDelayedMessageScheduler();

                cfg.ConfigureEndpoints(context);
            });
    });
}
