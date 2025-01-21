using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[Route("[controller]")]
[ApiController]
[Authorize]
public class HomeController : ControllerBase
{
    private readonly IServiceContext _serviceContext;

    public HomeController(IServiceContext serviceContext)
    {
        _serviceContext = serviceContext;
    }

    [HttpGet("Index")]
    public async Task<IActionResult> Index()
    {
        return Ok(await _serviceContext.HomeService.GetHome());
    }



}
