using Application.DTO.Request;
using Application.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[Route("[controller]")]
[ApiController]
public class CostCenterController : ControllerBase
{
    private readonly IServiceContext _serviceContext;
    public CostCenterController(IServiceContext serviceContext)
    {
        _serviceContext = serviceContext;
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _serviceContext.CostCenterService.GetAll());
    }

    [HttpGet("Get/{id}")]
    public async Task<IActionResult> Get(int id)
    {
        return Ok(await _serviceContext.CostCenterService.Get(id));
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create(CreateCostCenter DTO)
    {
        var result = await _serviceContext.CostCenterService.Create(DTO);
        if (!result.IsSucceed)
            return BadRequest(result);

        return Ok(result);
    }
    [HttpPut("Edit")]
    public async Task<IActionResult> Edit(CreateCostCenter DTO)
    {
        var result = await _serviceContext.CostCenterService.Edit(DTO);
        if (!result.IsSucceed)
            return BadRequest(result);

        return Ok(result);
    }


    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _serviceContext.CostCenterService.Delete(id);
        if (!result.IsSucceed)
            return BadRequest(result);

        return Ok(result);
    }
}
