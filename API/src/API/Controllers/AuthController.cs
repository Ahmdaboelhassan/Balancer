using Domain.DTO.Request;
using Domain.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[Route("[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginDTO model)
    {
        var result = await _authService.Login(model);
        return result.IsAuth ? Ok(result) : BadRequest(result);

    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register (LoginDTO model)
    {
        var result = await _authService.Register(model);
        return result.IsAuth ? Ok(result) : BadRequest(result);
    }

    [HttpPost("RefreshToken")]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequest refreshToken)
    {
        var result = await _authService.RefreshToken(refreshToken.RefreshToken);
        return result.IsAuth ? Ok(result) : BadRequest(result);
    }

}
