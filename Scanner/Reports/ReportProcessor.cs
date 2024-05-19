using Microsoft.Azure.Cosmos;
using Scanner.Models;
using Scanner.Service;
using NameServer = Scanner.Reports.Types.NameServer;

namespace Scanner.Reports;

public class ReportProcessor(Database database, DnsService dns, IpinfoService ipinfo) : BackgroundService
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

        try
        {
            report.NameServers = dns.GetNameServers(report.DomainName)
                .Select(addr => new NameServer
                {
                    Hostname = addr,
                    Service = NameserverLookupService.GetService(addr)
                }).ToArray();

            var apex = dns.GetApexAddresses(report.DomainName);
            report.ApexAddresses = apex.Select(addr => ipinfo.GetIpInfo(addr).Result).ToArray();

            report.ApexText = dns.GetApexTextRecords(report.DomainName);
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
            await database.GetContainer(AppConstants.ReportsContainer).ReplaceItemAsync(report, report.Id);
        }
    }
}