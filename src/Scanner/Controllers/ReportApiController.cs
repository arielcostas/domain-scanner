using Microsoft.AspNetCore.Mvc;
using Scanner.Service;
using Scanner.ViewModels;

namespace Scanner.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportApiController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportApiController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("")]
    public IActionResult ListRecent()
    {
        var reports = _reportService.ListAllReports(10, 0);
        return Ok(new ListReportsViewModel { Reports = reports });
    }
}