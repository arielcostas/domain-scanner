using Newtonsoft.Json;
using Scanner.Reports.Types;

namespace Scanner.Models;

public class Report
{
    [JsonProperty("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string DomainName { get; set; }
    public string? Error { get; set; }

    public string[] NameServers { get; set; } = [];
    public Address[] ApexAddresses { get; set; } = [];
    public string[] ApexText { get; set; } = [];
    
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public ReportStatus Status { get; set; } = ReportStatus.Created;
}

public enum ReportStatus
{
    Created,
    Processing,
    Failed,
    Completed
}