using CosmosDbBackup.Application;
using CosmosDbBackup.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

public class HomeController : Controller
{
    private readonly ICosmosDbBackupService _cosmosDbBackupService;
    private readonly ApiConfiguration _apiConfiguration;

    public HomeController(ICosmosDbBackupService cosmosDbBackupService, IOptions<ApiConfiguration> apiConfiguration)
    {
        _cosmosDbBackupService = cosmosDbBackupService;
        _apiConfiguration = apiConfiguration.Value;
    }

    public IActionResult Index()
    {

        var viewModel = new HomeIndexViewModel
        {
            AccountNames = _apiConfiguration.CosmosDbConnectionStrings.GetAccountNames().ToList(),
        };
        return View(viewModel);
    }

    // GET: api/CosmosDb/Databases?accountName=account1
    [HttpGet("Databases")]
    public async Task<ActionResult<IEnumerable<string>>> GetDatabases(string accountName)
    {
        if (string.IsNullOrWhiteSpace(accountName))
        {
            return BadRequest("Account name must be provided.");
        }

        try
        {
            var databases = await _cosmosDbBackupService.GetDatabaseNamesAsync(accountName);
            return Ok(databases);
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Account with name '{accountName}' not found.");
        }
    }

    // GET: api/CosmosDb/Collections?accountName=account1&databaseName=database1
    [HttpGet("Collections")]
    public async Task<ActionResult<IEnumerable<string>>> GetCollections(string accountName, string databaseName)
    {
        if (string.IsNullOrWhiteSpace(accountName) || string.IsNullOrWhiteSpace(databaseName))
        {
            return BadRequest("Account and database names must be provided.");
        }

        try
        {
            var collections = await _cosmosDbBackupService.GetCollectionNamesAsync(accountName, databaseName);
            return Ok(collections);
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Database with name '{databaseName}' in account '{accountName}' not found.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> DownloadDocuments(string accountName, string databaseName, string collectionName)
    {
        var stream = await _cosmosDbBackupService.DownloadDocuments(accountName, databaseName, collectionName);
        return File(stream, "application/zip", $"{accountName}_{databaseName}_{collectionName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.zip");
    }
}

internal class HomeIndexViewModel
{
    public IEnumerable<string> AccountNames { get; set; } = [];
}