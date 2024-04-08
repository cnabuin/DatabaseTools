using Microsoft.Azure.Cosmos;
using System.Collections.Concurrent;

namespace CosmosDbBackup.Infrastructure;

public sealed class CosmosClientFactory : ICosmosClientFactory, IDisposable
{
    private readonly ConcurrentDictionary<string, Lazy<CosmosClient>> _clients = new();

    public CosmosClientFactory()
    {
    }

    public void RegisterCosmosClient(string accountName, string connectionString)
    {
        _clients.TryAdd(accountName, new Lazy<CosmosClient>(() => new CosmosClient(connectionString), LazyThreadSafetyMode.ExecutionAndPublication));
    }

    public CosmosClient GetCosmosClient(string accountName)
    {
        return _clients.TryGetValue(accountName, out Lazy<CosmosClient>? lazyClient)
            ? lazyClient.Value
            : throw new ArgumentException($"No CosmosClient registered for the account '{accountName}'.");
    }

    public void Dispose()
    {
        foreach (Lazy<CosmosClient> client in _clients.Values)
        {
            if (client.IsValueCreated)
            {
                try
                {
                    client.Value.Dispose();
                }
                catch (Exception)
                {
                    //ignore
                }
            }
        }
    }

    public IDictionary<string, CosmosClient> GetAllClients()
    {
        throw new NotImplementedException();
    }
}