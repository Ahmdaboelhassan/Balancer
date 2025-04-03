using Domain.DTO.Request;
using Domain.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[Route("[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IServiceContext _serviceContext;

    public AuthController(IServiceContext serviceContext)
    {
        _serviceContext = serviceContext;
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginDTO model)
    {
        var result = await _serviceContext.AuthService.Login(model);
        return result.IsAuth ? Ok(result) : BadRequest(result);

    }

    [Authorize]
    [HttpPost("Register")]
    public async Task<IActionResult> Register (LoginDTO model)
    {
        var result = await _serviceContext.AuthService.Register(model);
        return result.IsAuth ? Ok(result) : BadRequest(result);
    }

}
