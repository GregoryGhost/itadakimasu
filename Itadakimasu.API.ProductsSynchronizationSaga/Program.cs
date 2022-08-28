using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Grpc.Net.Client;

using Itadakimasu.API.ProductsSynchronizationSaga.Types.Configs;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using MassTransit;

using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);


var rabbitMqConfig = ConfigHelper.GetRabbitMqConfig();

builder.Services.AddMassTransit(
    configurator =>
    {
        configurator.AddDelayedMessageScheduler();

        configurator.SetKebabCaseEndpointNameFormatter();

        // By default, sagas are in-memory, but should be changed to a durable
        // saga repository.
        configurator.SetInMemorySagaRepositoryProvider();

        var entryAssembly = Assembly.GetEntryAssembly();

        configurator.AddConsumers(entryAssembly);
        configurator.AddSagaStateMachines(entryAssembly);
        configurator.AddSagas(entryAssembly);
        configurator.AddActivities(entryAssembly);

        configurator.UsingRabbitMq(
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

var microservicesConfig = ConfigHelper.GetMicroservicesConfig();

builder.Services.AddSingleton(
    s =>
    {
        var channel = GrpcChannel.ForAddress(microservicesConfig.ApiProductsAggregatorAddress);

        var client = new ProductsProxy.V1.ProductsProxy.ProductsProxyClient(channel);

        return client;
    });

builder.Services.AddSingleton(
    s =>
    {
        var channel = GrpcChannel.ForAddress(microservicesConfig.ApiProductsSynchronizerAddress);

        var client = new ProductsSyncer.V1.ProductsSyncer.ProductsSyncerClient(channel);

        return client;
    });


var app = builder.Build();

app.Run();