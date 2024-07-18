using Microsoft.AspNetCore.Mvc;
using Scanner.Service;
using Scanner.Responses;

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
    public IActionResult ListRecent([FromQuery] int page, [FromQuery] int pageSize = 10)
    {
        if (!ValidateLimitOffset(page, pageSize, out var message))
        {
            return BadRequest(message);
        }

        var reports = _reportService.ListAllReports(pageSize, (page - 1) * pageSize);
        return Ok(new ListReportsResponse
        {
            Reports = reports,
            Total = _reportService.CountAllReports()
        });
    }

    [HttpGet("{domain}")]
    public IActionResult ListByDomain(
        [FromRoute] string domain,
        [FromQuery] int page,
        [FromQuery] int pageSize = 10
    )
    {
        if (!ValidateLimitOffset(page, pageSize, out var message))
        {
            return BadRequest(message);
        }

        if (!domain.EndsWith('.')) domain += ".";

        var reports = _reportService.ListReportsByDomain(domain, pageSize, (page - 1) * pageSize);

        return Ok(new ListReportsBydomainResponse
        {
            Reports = reports,
            Domain = domain,
            Total = _reportService.CountReportsByDomain(domain)
        });
    }

    [HttpGet("{domain}/{id}")]
    public IActionResult Details(string domain, string id)
    {
        var report = _reportService.GetReport(domain, id);
        if (report is null) return NotFound();
        return Ok(new ReportDetailsResponse { Report = report });
    }

    [HttpPost("")]
    public IActionResult Create([FromForm] string domain)
    {
        _reportService.CreateReport(domain);
        return Ok();
    }

    private static bool ValidateLimitOffset(int page, int pageSize, out string? message)
    {
        if (page < 1)
        {
            message = "Invalid page number.";
            return false;
        }

        if (pageSize < 1)
        {
            message = "Invalid page size.";
            return false;
        }

        if (pageSize > 100)
        {
            message = "Page size limit is 100.";
            return false;
        }

        message = null;
        return true;
    }
}