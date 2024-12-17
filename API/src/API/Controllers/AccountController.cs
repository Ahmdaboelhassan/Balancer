using Application.DTO.Request;
using Application.DTO.Response;
using Application.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[Route("[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IServiceContext _serviceContext;
    public AccountController(IServiceContext serviceContext)
    {
        _serviceContext = serviceContext;
    }

    [HttpGet("Get/{id}")]
    public async Task<IActionResult> Get(int id)
    {
        return Ok(await _serviceContext.AccountService.GetById(id));
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _serviceContext.AccountService.GetAll());
    }

    [HttpGet("GetSelectList")]
    public async Task<IActionResult> GetSelectList()
    {
        return Ok(await _serviceContext.AccountService.GetSelectList());
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create(CreateAccountDTO DTO)
    {
        var result = await _serviceContext.AccountService.Create(DTO);
        if (!result.IsSucceed)
            return BadRequest(result.Message);

        return Ok(result.Message);
    }
    [HttpPut("Edit")]
    public async Task<IActionResult> Edit(CreateAccountDTO DTO)
    {
        var result = await _serviceContext.AccountService.Edit(DTO);
        if (!result.IsSucceed)
            return BadRequest(result.Message);

        return Ok(result.Message);
    }
    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _serviceContext.AccountService.Delete(id);
        if (!result.IsSucceed)
            return BadRequest(result.Message);

        return Ok(result.Message);
    }

}
