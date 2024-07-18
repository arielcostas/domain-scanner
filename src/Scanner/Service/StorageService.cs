using DnsClient;
using Microsoft.Azure.Cosmos;
using Scanner.Models;

namespace Scanner.Service;

public class CosmosReportService : IReportService
{
    private readonly Database _database;
    private readonly DnsService _dns;

    public CosmosReportService(Database database, DnsService dns)
    {
        _database = database;
        _dns = dns;
    }

    public List<ListReportsItem> ListAllReports(int limit = 10, int offset = 0)
    {
        var container = _database.GetContainer(AppConstants.ReportsContainer);
        const string queryText = "SELECT c.id, c.DomainName, c.RequestedAt, c.Status FROM c ORDER BY c.RequestedAt DESC OFFSET @offset LIMIT @limit";
        var queryDefinition = new QueryDefinition(queryText)
            .WithParameter("@offset", offset)
            .WithParameter("@limit", limit);

        using var iterator = container.GetItemQueryIterator<Report>(queryDefinition);

        return GetReportsFromIterator(iterator);
    }

    public int CountAllReports()
    {
        var container = _database.GetContainer(AppConstants.ReportsContainer);
        var queryDefinition = new QueryDefinition("SELECT VALUE COUNT(1) FROM c");
        var iterator = container.GetItemQueryIterator<int>(queryDefinition);
        return iterator.ReadNextAsync().Result.Resource.First();
    }

    public List<ListReportsItem> ListReportsByDomain(string domain, int limit = 10, int offset = 0)
    {
        var container = _database.GetContainer(AppConstants.ReportsContainer);
        const string queryText = "SELECT c.id, c.DomainName, c.RequestedAt, c.Status FROM c ORDER BY c.RequestedAt DESC OFFSET @offset LIMIT @limit";
        var queryDefinition = new QueryDefinition(queryText)
            .WithParameter("@offset", offset)
            .WithParameter("@limit", limit);
        
        var iterator = container.GetItemQueryIterator<Report>(queryDefinition, requestOptions: new QueryRequestOptions
        {
            PartitionKey = new PartitionKey(domain)
        });
        
        return GetReportsFromIterator(iterator);
    }

    public int CountReportsByDomain(string domain)
    {
        var container = _database.GetContainer(AppConstants.ReportsContainer);
        var queryDefinition = new QueryDefinition("SELECT VALUE COUNT(1) FROM c");
        var iterator = container.GetItemQueryIterator<int>(queryDefinition, requestOptions: new QueryRequestOptions
        {
            PartitionKey = new PartitionKey(domain)
        });
        return iterator.ReadNextAsync().Result.Resource.First();
    }

    private List<ListReportsItem> GetReportsFromIterator(FeedIterator<Report> iterator)
    {
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

        return reports;
    }
    
    public Report GetReport(string domain, string id)
    {
        var container = _database.GetContainer(AppConstants.ReportsContainer);
        return container.ReadItemAsync<Report>(id, new PartitionKey(domain)).Result.Resource;
    }
    
    public void CreateReport(string domain)
    {
        domain = DnsString.Parse(domain).Value;
        if (_dns.PrevalidateDomain(domain) == false)
        {
            throw new ArgumentException("Domain does not exist or is unreachable.");
        }
        var container = _database.GetContainer(AppConstants.ReportsContainer);
        container.CreateItemAsync(new Report { DomainName = domain }, new PartitionKey(domain)).Wait();
    }
}

public interface IReportService
{
    List<ListReportsItem> ListAllReports(int limit, int offset);
    int CountAllReports();
    List<ListReportsItem> ListReportsByDomain(string domain, int limit, int offset);
    int CountReportsByDomain(string domain);
    Report GetReport(string domain, string id);
    void CreateReport(string domain);
}

public record ListReportsItem(string Id, string DomainName, DateTime RequestedAt, ReportStatus Status);