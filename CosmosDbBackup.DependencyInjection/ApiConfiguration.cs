namespace CosmosDbBackup.DependencyInjection;

public class ApiConfiguration
{
    /// <summary>
    /// Key: Account Name <br/>
    /// Value: Connection String <br/>
    /// </summary>
    public CosmosDbConnectionStringDictionary CosmosDbConnectionStrings { get; set; } = [];
}
