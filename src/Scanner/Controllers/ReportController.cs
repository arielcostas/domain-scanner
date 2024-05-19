using Microsoft.AspNetCore.Mvc;
using Scanner.Service;
using Scanner.ViewModels;

namespace Scanner.Controllers;

[Controller]
[Route("reports")]
public class ReportController : Controller
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("")]
    public IActionResult ListRecent()
    {
        var reports = _reportService.ListAllReports(10, 0);
        return View(new ListReportsViewModel { Reports = reports });
    }

    [HttpGet("{domain}")]
    public IActionResult ListBydomain(string domain)
    {
        var reports = _reportService.ListReportsByDomain(domain, 10, 0);
        return View(new ListReportsBydomainViewModel { Reports = reports, Domain = domain });
    }

    [HttpGet("{domain}/{id}")]
    public IActionResult Details(string domain, string id)
    {
        var report = _reportService.GetReport(domain, id);
        return View(new ReportDetailsViewModel { Report = report });
    }

    [HttpPost("")]
    public IActionResult Create(string domain)
    {
        _reportService.CreateReport(domain);
        return RedirectToAction("ListBydomain", new { domain });
    }
}