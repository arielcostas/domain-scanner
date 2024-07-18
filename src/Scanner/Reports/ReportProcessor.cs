using Microsoft.Azure.Cosmos;
using Scanner.Models;
using Scanner.Service;
using NameServer = Scanner.Reports.Types.NameServer;

namespace Scanner.Reports;

public class ReportProcessor(Database database, DnsService dns, IpinfoService ipinfo, ILogger<ReportProcessor> logger) : BackgroundService
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
        logger.LogInformation("{ReportId} Processing report", report.Id);
        report.Status = ReportStatus.Processing;
        await database.GetContainer(AppConstants.ReportsContainer).ReplaceItemAsync(report, report.Id);
        logger.LogInformation("{ReportId} report updated to processing", report.Id);

        try
        {
            logger.LogInformation("{ReportId} getting nameservers and apex addresses for {DomainName}", report.Id, report.DomainName);
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
            logger.LogInformation("{ReportId} report completed", report.Id);
        }
        catch (Exception e)
        {
            report.Status = ReportStatus.Failed;
            report.Error = e.Message;
            logger.LogError(e, "{ReportId} error processing report", report.Id);
        }
        finally
        {
            // When the report was finalized, either successfully or not
            report.CompletedAt = DateTime.UtcNow;
            await database.GetContainer(AppConstants.ReportsContainer).ReplaceItemAsync(report, report.Id);
        }
    }
}