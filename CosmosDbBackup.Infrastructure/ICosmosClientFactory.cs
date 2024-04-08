using Microsoft.Azure.Cosmos;

namespace CosmosDbBackup.Infrastructure;

public interface ICosmosClientFactory
{
    CosmosClient GetCosmosClient(string accountName);
}
