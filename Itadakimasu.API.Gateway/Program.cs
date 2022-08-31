using System.Reflection;

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
    builder.Services.AddSingleton<ProductsSynchronizationMapper>();
}

static void RegisterGraphQlTypes(WebApplicationBuilder builder)
{
    builder.Services
           .AddGraphQLServer()
           .AddInMemorySubscriptions()
           
           .AddQueryType(t => t.Name("Query"))
           .AddTypeExtension<ProductsQuery>()
           .AddTypeExtension<ProductsSynchronizationQuery>()
           
           .AddMutationType(t => t.Name("Mutation"))
           .AddTypeExtension<ProductsMutation>()
           .AddTypeExtension<ProductsSynchronizationMutation>()
           
           .AddSubscriptionType(t => t.Name("Subscription"))
           .AddTypeExtension<ProductsSubscription>()
           .AddTypeExtension<ProductsSycnhronizationSubscription>();
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
    var isRunningInContainer = ConfigHelper.CheckRunningInContainer();
    
    builder.Services.AddMassTransit(configurator =>
    {
        var entryAssembly = Assembly.GetEntryAssembly();
        configurator.AddConsumers(entryAssembly);
        
        configurator.UsingRabbitMq(
            (context, cfg) =>
            {
                if (isRunningInContainer)
                {
                    cfg.Host("rabbitmq");
                }
                else
                {
                    cfg.Host(rabbitMqConfig.Address, "/", h =>
                    {
                        h.Username(rabbitMqConfig.Login);
                        h.Password(rabbitMqConfig.Password);
                    });
                }

                cfg.UseDelayedMessageScheduler();

                cfg.ConfigureEndpoints(context);
            });
    });
}
