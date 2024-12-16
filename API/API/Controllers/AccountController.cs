using Application.DTO.Request;
using Application.DTO.Response;
using Application.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IServiceContext _serviceContext;
    public AccountController(IServiceContext serviceContext)
    {
        _serviceContext = serviceContext;
    }

    [HttpGet("GetAccount/{id}")]
    public async Task<IActionResult> GetAccount(int id)
    {
        return Ok(await _serviceContext.AccountService.GetAccountById(id));
    }

    [HttpGet("GetAllAccounts")]
    public async Task<IActionResult> GetAllAccounts()
    {
        return Ok(await _serviceContext.AccountService.GetAllAccounts());
    }

    [HttpGet("AccountsSelect")]
    public async Task<IActionResult> AccountsSelect()
    {
        return Ok(await _serviceContext.AccountService.GetAccountSelectList());
    }

    [HttpPost("CreateAccount")]
    public async Task<IActionResult> CreateAccount(CreateAccountDTO DTO)
    {
        var result = await _serviceContext.AccountService.CreateAccount(DTO);
        if (!result.IsSucceed)
            return BadRequest(result.Message);

        return Ok(result.Message);
    }
    [HttpPut("EditAccount")]
    public async Task<IActionResult> EditAccount(CreateAccountDTO DTO)
    {
        var result = await _serviceContext.AccountService.EditAccount(DTO);
        if (!result.IsSucceed)
            return BadRequest(result.Message);

        return Ok(result.Message);
    }
    [HttpDelete("DeleteAccount/{id}")]
    public async Task<IActionResult> DeleteAccount(int id)
    {
        var result = await _serviceContext.AccountService.DeleteAccount(id);
        if (!result.IsSucceed)
            return BadRequest(result.Message);

        return Ok(result.Message);
    }

}
