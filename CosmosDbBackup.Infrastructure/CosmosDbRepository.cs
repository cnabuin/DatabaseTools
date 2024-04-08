using System.Collections.Concurrent;
using CosmosDbBackup.Application;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace CosmosDbBackup.Infrastructure;

public class CosmosDbRepository : ICosmosDbRepository
{
    private readonly ICosmosClientFactory _cosmosClientFactory;
    private readonly ConcurrentDictionary<string, IEnumerable<string>> _databaseNamesCache = new ConcurrentDictionary<string, IEnumerable<string>>();
    private readonly ConcurrentDictionary<(string, string), IEnumerable<string>> _collectionNamesCache = new ConcurrentDictionary<(string, string), IEnumerable<string>>();

    public CosmosDbRepository(ICosmosClientFactory cosmosClientFactory)
    {
        _cosmosClientFactory = cosmosClientFactory;
    }

    public async Task<IEnumerable<dynamic>> GetAllDocumentsFromCollection(string accountName, string databaseName, string collectionName, CancellationToken cancellationToken = default)
    {
        var documents = new List<dynamic>();
        var cosmosClient = _cosmosClientFactory.GetCosmosClient(accountName);
        var container = cosmosClient.GetContainer(databaseName, collectionName);
        using var iterator = container.GetItemLinqQueryable<dynamic>().ToFeedIterator();
        while (iterator.HasMoreResults)
        {
            FeedResponse<dynamic> response = await iterator.ReadNextAsync(cancellationToken);
            documents.AddRange(response.Resource);
        }

        return documents;
    }

    public async Task<IEnumerable<string>> GetDatabaseNamesAsync(string accountName)
    {
        if (!_databaseNamesCache.TryGetValue(accountName, out var databaseNames))
        {
            var client = _cosmosClientFactory.GetCosmosClient(accountName);
            var iterator = client.GetDatabaseQueryIterator<DatabaseProperties>();
            var databases = new List<string>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                databases.AddRange(response.Select(db => db.Id));
            }

            databaseNames = databases;
            _databaseNamesCache.TryAdd(accountName, databaseNames);
        }

        return databaseNames;
    }

    public async Task<IEnumerable<string>> GetCollectionNamesAsync(string accountName, string databaseName)
    {
        var cacheKey = (accountName, databaseName);
        if (!_collectionNamesCache.TryGetValue(cacheKey, out var collectionNames))
        {
            var client = _cosmosClientFactory.GetCosmosClient(accountName);
            var database = client.GetDatabase(databaseName);
            var iterator = database.GetContainerQueryIterator<ContainerProperties>();
            var collections = new List<string>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                collections.AddRange(response.Select(container => container.Id));
            }

            collectionNames = collections;
            _collectionNamesCache.TryAdd(cacheKey, collectionNames);
        }

        return collectionNames;
    }
}