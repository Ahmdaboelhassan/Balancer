
using Domain.DTO.Request;
using Domain.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[Route("[controller]")]
[ApiController]
[Authorize]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet("Get/{id}")]
    public async Task<IActionResult> Get(int id)
    {
        return Ok(await _accountService.GetById(id));
    }
    [HttpGet("Search")]
    public async Task<IActionResult> Get(string criteria)
    {
        return Ok(await _accountService.Search(criteria));
    }
    [HttpGet("GetBalance/{id}")]
    public async Task<IActionResult> GetBalance(int id)
    {
        return Ok(new { balance =  await _accountService.GetBalance(id)});
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _accountService.GetAll());
    }
    [HttpGet("GetPrimaryAccounts")]
    public async Task<IActionResult> GetPrimaryAccounts()
    {
        return Ok(await _accountService.GetPrimaryAccounts());
    }
    [HttpGet("GetChilds/{id}")]
    public async Task<IActionResult> GetChilds(int id)
    {
        return Ok(await _accountService.GetChilds(id));
    }

    [HttpGet("GetSelectList")]
    public async Task<IActionResult> GetSelectList()
    {
        return Ok(await _accountService.GetSelectList());
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create(CreateAccountDTO DTO)
    {
        var result = await _accountService.Create(DTO);
        if (!result.IsSucceed)
            return BadRequest(result);

        return Ok(result);
    }
    [HttpPut("Edit")]
    public async Task<IActionResult> Edit(CreateAccountDTO DTO)
    {
        var result = await _accountService.Edit(DTO);
        if (!result.IsSucceed)
            return BadRequest(result);

        return Ok(result);
    }
    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _accountService.Delete(id);
        if (!result.IsSucceed)
            return BadRequest(result);

        return Ok(result);
    }

}
