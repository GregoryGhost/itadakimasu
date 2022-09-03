using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Grpc.Net.Client;

using Itadakimasu.API.ProductsSynchronizationSaga.Services;
using Itadakimasu.API.ProductsSynchronizationSaga.Types.Configs;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using MassTransit;

using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

var microservicesConfig = ConfigHelper.GetMicroservicesConfig();

var rabbitMqConfig = ConfigHelper.GetRabbitMqConfig();
var isRunningInContainer = ConfigHelper.CheckRunningInContainer();

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

builder.Services.AddGrpcClient<ProductsProxy.V1.ProductsProxy.ProductsProxyClient>(
           options =>
           {
               options.Address = new Uri(microservicesConfig.ApiProductsAggregatorAddress);
           })
       .ConfigureChannel(
           options => { options.UnsafeUseInsecureChannelCallCredentials = true; });

builder.Services.AddGrpcClient<ProductsSyncer.V1.ProductsSyncer.ProductsSyncerClient>(
           options =>
           {
               options.Address = new Uri(microservicesConfig.ApiProductsSynchronizerAddress);
           })
       .ConfigureChannel(
           options => { options.UnsafeUseInsecureChannelCallCredentials = true; });

builder.Services.AddHostedService<ProductsAggregatorNotifier>();

var app = builder.Build();

app.Run();