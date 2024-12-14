using Application.DTO.Request;
using Application.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PeriodController : ControllerBase
{
    public readonly IServiceContext _services;

    public PeriodController(IServiceContext serviceContext)
    {
        _services = serviceContext;
    }

    [HttpGet("Periods")]
    public async Task<IActionResult> Periods(DateTime? from  , DateTime? to)
    {
        return Ok(await _services.PeriodService.GetAllPeriods(from, to));
    }

    [HttpGet("PeriodSelectList")]
    public async Task<IActionResult> PeriodSelectList()
    {
        return Ok(await _services.PeriodService.GetAllPeriodSelectList());
    }

    [HttpGet("NewPeriod")]
    public async Task<IActionResult> NewPeriod()
    {
        return Ok(await _services.PeriodService.GetNewPeriod());
    }

    [HttpGet("Period/{id}")]
    public async Task<IActionResult> Period(int id)
    {
        return Ok(await _services.PeriodService.GetPeriodById(id));
    }


    [HttpPost("CreatePeriod")]
    public async Task<IActionResult> CreatePeriod(SavePeriodDTO DTO)
    {
        var result = await _services.PeriodService.CreatePeriod(DTO);

        if (result.IsSucceed)
            return Ok(result.Message);

        return BadRequest(result.Message);
    }

    [HttpPut("EditPeriod")]
    public async Task<IActionResult> EditPeriod(SavePeriodDTO DTO)
    {
        var result = await _services.PeriodService.EditPeriod(DTO);

        if (result.IsSucceed)
            return Ok(result.Message);

        return BadRequest(result.Message);
    }

    [HttpDelete("DeletePeriod/{id}")]
    public async Task<IActionResult> DeletePeriod(int id)
    {
        var result = await _services.PeriodService.DeletePeriod(id);

        if (result.IsSucceed)
            return Ok(result.Message);

        return BadRequest(result.Message);
    }



}
