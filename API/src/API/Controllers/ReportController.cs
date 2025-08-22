using Domain.Enums;
using Domain.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[Route("[controller]")]
[ApiController]
[Authorize]
public class ReportController : ControllerBase
{

    private readonly IServiceContext _serviceContext;
    public ReportController(IServiceContext serviceContext)
    {
        _serviceContext = serviceContext;
    }

    [HttpGet("AccountStatement")]
    public async Task<IActionResult> AccountStatement(DateTime? from , DateTime? to, int account , int? costCenter , bool openingBalance)
    {
        return Ok(await _serviceContext.ReportService.GetAccountStatement(from , to , account , costCenter , openingBalance));
    }

    [HttpGet("IncomeStatement")]
    public async Task<IActionResult> IncomeStatement(DateTime from, DateTime to)
    {
        return Ok(await _serviceContext.ReportService.GetIncomeStatement(from, to));
    }
    [HttpGet("AccountsSummary")]
    public async Task<IActionResult> AccountsSummary(DateTime from, DateTime to)
    {
        return Ok(await _serviceContext.ReportService.GetAccountsSummary(from, to));
    }
    [HttpGet("AccountsOverview")]
    public async Task<IActionResult> AccountsOverview(DateTime from, DateTime to, int? maxLevel)
    {
        return Ok(await _serviceContext.ReportService.GetAccountsOverview(from, to, maxLevel));
    }

    [AllowAnonymous]
    [HttpGet("AccountComparer")]
    public async Task<IActionResult> AccountComparer(DateTime? from, DateTime? to, int account, int? costCenter, int groupType)
    {
        return Ok(await _serviceContext.ReportService.GetAccountComparer(from, to ,account , costCenter , (AccountComparerGroups)groupType));
    }
}
