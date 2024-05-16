using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Scanner.Models;
using Scanner.Reports.Fetchers;

namespace Scanner.Controllers;

[Controller]
public class ReportController : Controller
{
    private readonly DnsFetcher _dnsFetcher;
    private readonly Database _database;

    public ReportController(Database database)
    {
        _database = database;
        _dnsFetcher = new DnsFetcher();
    }

    [HttpGet("")]
    public IActionResult List()
    {
        var container = _database.GetContainer(AppConstants.ReportsContainer);
        using var iterator = container.GetItemQueryIterator<Report>(
            queryText: "SELECT * FROM c",
            requestOptions: new QueryRequestOptions { MaxItemCount = 100 }
        );
        
        var reports = new List<ListReportsItem>();
        while (iterator.HasMoreResults)
        {
            var response = iterator.ReadNextAsync().Result;
            reports.AddRange(response.Resource.Select(r => new ListReportsItem(
                r.Id,
                r.DomainName,
                r.RequestedAt,
                r.Status
            )));
        }

        return View(reports);
    }
    
    [HttpGet("{domain}/{id}")]
    public async Task<IActionResult> Details(string domain, string id)
    {
        var container = _database.GetContainer(AppConstants.ReportsContainer);
        var report = await container.ReadItemAsync<Report>(id, new PartitionKey(domain));
        
        return View(report.Resource);
    }
    
    [HttpPost("")]
    public async Task<IActionResult> Create(string domain)
    {
        var report = new Report { DomainName = domain };
        
        var container = _database.GetContainer(AppConstants.ReportsContainer);
        await container.CreateItemAsync(report, new PartitionKey(domain));
        return RedirectToAction("List");
    }

    public record ListReportsItem(string Id, string DomainName, DateTime RequestedAt, ReportStatus Status);
}