using Microsoft.Azure.Cosmos;
using Scanner.Models;
using Scanner.Reports.Fetchers;
using IpinfoFetcher = Scanner.Reports.Fetchers.IpinfoFetcher;

namespace Scanner.Reports;

public class ReportProcessor(Database database) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var sourceContainer = database.GetContainer(AppConstants.ReportsContainer);
        var leaseContainer = database.GetContainer(AppConstants.ReportsLeaseContainer);

        var processor = sourceContainer
            .GetChangeFeedProcessorBuilder<Report>("report", OnChangesReceived)
            .WithInstanceName($"Scanner-{Environment.MachineName}")
            .WithLeaseContainer(leaseContainer)
            .Build();

        await processor.StartAsync();
        stoppingToken.Register(() => processor.StopAsync());
    }

    private Task OnChangesReceived(
        IReadOnlyCollection<Report> readOnlyCollection,
        CancellationToken cancellationToken
    )
    {
        return Task.WhenAll(
            readOnlyCollection.Where(r => r.Status == ReportStatus.Created).Select(ProcessReport)
        );
    }

    private async Task ProcessReport(Report report)
    {
        report.Status = ReportStatus.Processing;
        await database.GetContainer(AppConstants.ReportsContainer).ReplaceItemAsync(report, report.Id);

        DnsFetcher dnsFetcher = new();
        IpinfoFetcher ipinfoFetcher = new();

        try
        {
            report.NameServers = dnsFetcher.GetNameServers(report.DomainName);
            report.ApexAddresses = dnsFetcher.GetApexAddresses(report.DomainName)
                .Select(addr => ipinfoFetcher.GetIpInfo(addr).Result).ToArray();

            report.ApexText = dnsFetcher.GetApexTextRecords(report.DomainName);
            report.Status = ReportStatus.Completed;
        }
        catch (Exception e)
        {
            report.Status = ReportStatus.Failed;
            report.Error = e.Message;
        }
        finally
        {
            // When the report was finalized, either successfully or not
            report.CompletedAt = DateTime.UtcNow;
        }

        await database.GetContainer(AppConstants.ReportsContainer).ReplaceItemAsync(report, report.Id);
    }
}