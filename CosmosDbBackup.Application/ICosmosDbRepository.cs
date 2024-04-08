namespace CosmosDbBackup.Application;

public interface ICosmosDbRepository
{
    Task<IEnumerable<dynamic>> GetAllDocumentsFromCollection(string accountName, string databaseName, string collectionName, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetCollectionNamesAsync(string accountName, string databaseName);
    Task<IEnumerable<string>> GetDatabaseNamesAsync(string accountName);
}
