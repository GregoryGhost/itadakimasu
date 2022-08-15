﻿using System.Threading.Channels;

using Itadakimasu.API.ProductsAggregator.Models;
using Itadakimasu.API.ProductsAggregator.Services;
using Itadakimasu.ProductsAggregator.DAL;

using ProductScrapper.Contracts;
using ProductScrapper.Services;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
builder.Services.AddSingleton<MrTakoScrapper>();

AddSynchronizationChannel(builder);
AddScrappedProductsChannel(builder);

builder.Services.AddSingleton<ProductsSynchronizationNotifier>();

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