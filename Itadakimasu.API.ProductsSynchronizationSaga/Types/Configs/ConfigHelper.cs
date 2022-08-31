namespace Itadakimasu.API.ProductsSynchronizationSaga.Types.Configs;

using Microsoft.AspNetCore.Builder;

public static class ConfigHelper
{
    public static RabbitMqConfig GetRabbitMqConfig()
    {
        const string RabbitMqAddress = "RABBITMQ_ADDRESS";
        var rabbitmqAddress = Environment.GetEnvironmentVariable(RabbitMqAddress)
            ?? throw new ArgumentNullException($"Require to set parameter {RabbitMqAddress}");
        const string RabbitMqLogin = "RABBITMQ_LOGIN";
        var login = Environment.GetEnvironmentVariable(RabbitMqLogin)
            ?? throw new ArgumentNullException($"Require to set parameter {RabbitMqLogin}");
        const string RabbitMqPassword = "RABBITMQ_PASSWORD";
        var password = Environment.GetEnvironmentVariable(RabbitMqPassword)
            ?? throw new ArgumentNullException($"Require to set parameter {RabbitMqPassword}");
        var config = new RabbitMqConfig
        {
            Address = rabbitmqAddress,
            Login = login,
            Password = password
        };

        return config;
    }

    public static DependentMicroservicesAddresses GetMicroservicesConfig()
    {
        const string ApiProductsAggregatorAddress = "API_PRODUCTS_AGGREGATOR_ADDRESS";
        var apiProductsAggregatorAdress = Environment.GetEnvironmentVariable(ApiProductsAggregatorAddress)
                              ?? throw new ArgumentNullException($"Require to set parameter {ApiProductsAggregatorAddress}");
        const string ApiProductsSynchronizerAddress = "API_PRODUCTS_SYNCHRONIZER_ADDRESS";
        var apiProductsSynchronizerAddress = Environment.GetEnvironmentVariable(ApiProductsSynchronizerAddress)
                    ?? throw new ArgumentNullException($"Require to set parameter {ApiProductsSynchronizerAddress}");
        var config = new DependentMicroservicesAddresses
        {
            ApiProductsAggregatorAddress = apiProductsAggregatorAdress,
            ApiProductsSynchronizerAddress = apiProductsSynchronizerAddress
        };

        return config;
    }

    //TODO: remove duplication
    public static bool CheckRunningInContainer()
    {
        const string EnvironmentParameter = "DOTNET_RUNNING_IN_CONTAINER";
        var value = Environment.GetEnvironmentVariable(EnvironmentParameter);
        var isRunningInContainer = !string.IsNullOrEmpty(value) && bool.Parse(value);
        
        return isRunningInContainer;
    }
}