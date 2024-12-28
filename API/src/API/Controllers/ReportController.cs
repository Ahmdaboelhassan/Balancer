using Application.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[Route("[controller]")]
[ApiController]
public class ReportController : ControllerBase
{

    private readonly IServiceContext _serviceContext;
    public ReportController(IServiceContext serviceContext)
    {
        _serviceContext = serviceContext;
    }

    [HttpGet("AccountStatement")]
    public async Task<IActionResult> AccountStatement(DateTime from , DateTime to, int account , int? costCenter , bool openingBalance)
    {
        return Ok(await _serviceContext.ReportService.GetAccountStatement(from , to , account , costCenter , openingBalance));
    }
}
