using Scanner.Service;

namespace Scanner.ViewModels;

public class ListReportsBydomainViewModel : ViewModel
{
    public string Domain { get; set; }
    public List<ListReportsItem> Reports { get; set; }
}