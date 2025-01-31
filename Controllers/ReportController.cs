using Asp.Versioning;
using BudgetApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BudgetApi.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class ReportController(ReportService reportService) : ControllerBase
{
    [HttpGet("cashflow")]
    public async Task<IActionResult> GetCashFlowReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        return Ok(await reportService.GetCashFlow(startDate, endDate));
    }
}