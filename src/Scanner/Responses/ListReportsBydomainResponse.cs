using Scanner.Service;

namespace Scanner.Responses;

public class ListReportsBydomainResponse
{
    public required string Domain { get; set; }
    public required List<ListReportsItem> Reports { get; set; }
    public required int Total { get; set; }
}