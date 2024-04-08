using CosmosDbBackup.Application;
using CosmosDbBackup.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CosmosDbBackup.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration, string environment)
    {
        ApiConfiguration apiEnvironmentConfiguration = services.AddConfigurationSection<ApiConfiguration>(configuration, environment);
        services.AddApplicationServices();
        services.AddInfrastructureServices(apiEnvironmentConfiguration);
        return services;
    }

    public static TConfig AddConfigurationSection<TConfig>(this IServiceCollection services, IConfiguration configuration, string sectionName) where TConfig : class
    {
        IConfigurationSection configSection = configuration.GetSection(sectionName);
        TConfig configValue = configSection.Get<TConfig>() ?? throw new InvalidOperationException($"The configuration section '{sectionName}' is missing or invalid.");
        services.Configure<TConfig>(configSection);
        return configValue;
    }

    private static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICosmosDbBackupService, CosmosDbBackupService>();

        return services;
    }

    private static IServiceCollection AddInfrastructureServices(this IServiceCollection services, ApiConfiguration apiConfig)
    {

        services.AddSingleton<ICosmosClientFactory>(sp =>
        {
            CosmosClientFactory factory = new();
            foreach (var cosmosdbConnectionString in apiConfig.CosmosDbConnectionStrings)
            {
                factory.RegisterCosmosClient(cosmosdbConnectionString.AccountName, cosmosdbConnectionString.ConnectionString);
            }

            return factory;
        });

        services.AddScoped<ICosmosDbRepository, CosmosDbRepository>();

        return services;
    }
}