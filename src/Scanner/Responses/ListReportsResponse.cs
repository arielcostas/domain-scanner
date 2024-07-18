using Scanner.Service;

namespace Scanner.Responses;

public class ListReportsResponse
{
    public required List<ListReportsItem> Reports { get; set; }
    public required int Total { get; set; }

}