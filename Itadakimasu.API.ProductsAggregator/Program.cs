using System.Threading.Channels;

using AngleSharp;
using AngleSharp.Html.Parser;

using Itadakimasu.API.ProductsAggregator.Models;
using Itadakimasu.API.ProductsAggregator.Services;
using Itadakimasu.ProductsAggregator.DAL;

using Microsoft.EntityFrameworkCore;

using ProductScrapper.Contracts;
using ProductScrapper.Services;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

AddDbContext(builder);

AddMrTakoScrapper(builder);

AddSynchronizationChannel(builder);
AddScrappedProductsChannel(builder);

builder.Services.AddScoped<ProductsSynchronizationNotifier>();

builder.Services.AddHostedService<ProductsLoopScrapper>();

builder.Services.AddSingleton(
    s =>
    {
        var scrappers = s.GetServices<IProductWebSiteScrapper>().ToList();
        var availableScrappers = new AvailableScrappers
        {
            Scrappers = scrappers
        };

        return availableScrappers;
    });
builder.Services.AddSingleton<ProductsAggregatorScrapper>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<ProductsAggregatorService>();
app.MapGet(
    "/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

var env = app.Environment;
if (env.IsDevelopment())
    app.MapGrpcReflectionService();

app.Run();

static void AddSynchronizationChannel(WebApplicationBuilder builder)
{
    var synchronizationChannel = Channel.CreateUnbounded<SynchronizatingRestaurant>();
    var synchronizationWriter = new ProductsSynchronizationWriter
    {
        ChannelWriter = synchronizationChannel.Writer
    };
    var synchronizationReader = new ProductsSynchronizationReader
    {
        ChannelReader = synchronizationChannel.Reader
    };
    builder.Services.AddSingleton(synchronizationWriter);
    builder.Services.AddSingleton(synchronizationReader);
}

static void AddScrappedProductsChannel(WebApplicationBuilder builder)
{
    var scrappedChannel = Channel.CreateUnbounded<SynchronizingScrappedResult>();
    var scrappedWriter = new ProductsResultSynchronizationWriter
    {
        ChannelWriter = scrappedChannel.Writer
    };
    var scrappedReader = new ProductsResultSynchronizationReader
    {
        ChannelReader = scrappedChannel.Reader
    };
    builder.Services.AddSingleton(scrappedWriter);
    builder.Services.AddSingleton(scrappedReader);
}

static void AddMrTakoScrapper(WebApplicationBuilder builder)
{
    var scrappingSettings = new ScrappingSettings
    {
        ScrappingRestaurantUrls = new[]//TODO: init from db and migration
        {
            "https://mistertako.ru/menyu/wok",
            "https://mistertako.ru/menyu/wok?page=2",
            "https://mistertako.ru/menyu/bluda-fri",
            "https://mistertako.ru/menyu/deserty",
            "https://mistertako.ru/menyu/dobavki",
            "https://mistertako.ru/menyu/napitki",
            "https://mistertako.ru/menyu/osnovnye-bluda",
            "https://mistertako.ru/menyu/rolly",
            "https://mistertako.ru/menyu/rolly?page=2",
            "https://mistertako.ru/menyu/salaty",
            "https://mistertako.ru/menyu/supy"
        },
        ProductsScrapperType = ProductsScrapperType.MrTako
    };
    builder.Services.AddSingleton(scrappingSettings);
    builder.Services.AddHttpClient<MrTakoClient>();
    builder.Services.AddSingleton<MrTakoParser>();
    builder.Services.AddSingleton<HtmlParser>();
    builder.Services.AddSingleton<MrTakoScrapper>();
}

static void AddDbContext(WebApplicationBuilder builder)
{
    var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

    if (string.IsNullOrEmpty(connectionString))
    {
        throw new ArgumentOutOfRangeException(nameof(connectionString), "Provide database connection string.");
    }
    
    builder.Services.AddDbContext<AppDbContext>(
        options => options.UseNpgsql(connectionString));
}