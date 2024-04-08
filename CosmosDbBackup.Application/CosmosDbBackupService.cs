using System.IO.Compression;
using System.Text;
using Newtonsoft.Json;
namespace CosmosDbBackup.Application;

public class CosmosDbBackupService : ICosmosDbBackupService
{
    ICosmosDbRepository _cosmosRepository;

    public CosmosDbBackupService(ICosmosDbRepository cosmosRepository)
    {

        _cosmosRepository = cosmosRepository;

    }

    public async Task<Stream> DownloadDocuments(string accountName, string databaseName, string collectionName)
    {
        var documents = await _cosmosRepository.GetAllDocumentsFromCollection(accountName, databaseName, collectionName);

        var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            foreach (var document in documents)
            {
                string id = document.id;
                var entry = archive.CreateEntry($"{id}.json");

                using (var entryStream = entry.Open())
                using (var streamWriter = new StreamWriter(entryStream, Encoding.UTF8))
                {
                    string jsonString = JsonConvert.SerializeObject(document);
                    await streamWriter.WriteAsync(jsonString);
                    await streamWriter.FlushAsync();
                }
            }
        }

        memoryStream.Seek(0, SeekOrigin.Begin);
        return memoryStream;
    }

    public async Task<IEnumerable<string>> GetDatabaseNamesAsync(string accountName)
    {
        return await _cosmosRepository.GetDatabaseNamesAsync(accountName);
    }

    public async Task<IEnumerable<string>> GetCollectionNamesAsync(string accountName, string databaseName)
    {
        return await _cosmosRepository.GetCollectionNamesAsync(accountName, databaseName);
    }
}
