
namespace CosmosDbBackup.Application;

public interface ICosmosDbBackupService
{
    Task<Stream> DownloadDocuments(string accountName, string databaseName, string collectionName);
    Task<IEnumerable<string>> GetCollectionNamesAsync(string accountName, string databaseName);
    Task<IEnumerable<string>> GetDatabaseNamesAsync(string accountName);
}