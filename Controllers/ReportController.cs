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
    public async Task<IActionResult> GetCashFlowReport([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] int customerId)
    {
        DateTime start = startDate ?? DateTime.UtcNow.Date;

        DateTime end = endDate ?? DateTime.UtcNow.Date.AddDays(1).AddTicks(-1);

        return Ok(await reportService.GetCashFlow(start, end, customerId));
    }
}