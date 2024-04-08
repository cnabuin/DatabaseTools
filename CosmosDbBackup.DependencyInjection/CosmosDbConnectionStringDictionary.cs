namespace CosmosDbBackup.DependencyInjection;

public class CosmosDbConnectionStringDictionary : Dictionary<string, string>
{
    public IEnumerable<string> GetAccountNames()
    {
        return Keys ?? Enumerable.Empty<string>();
    }

    public string GetConnectionString(string accountName)
    {
        return this[accountName];
    }

    public new IEnumerator<(string AccountName, string ConnectionString)> GetEnumerator()
    {
        Enumerator dictionaryEnumerator = base.GetEnumerator();
        while (dictionaryEnumerator.MoveNext())
        {
            KeyValuePair<string, string> keyValuePair = dictionaryEnumerator.Current;
            yield return (keyValuePair.Key, keyValuePair.Value);
        }
    }
}
