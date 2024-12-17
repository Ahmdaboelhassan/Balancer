using Application.DTO.Request;
using Application.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[Route("[controller]")]
[ApiController]
public class PeriodController : ControllerBase
{
    public readonly IServiceContext _services;

    public PeriodController(IServiceContext serviceContext)
    {
        _services = serviceContext;
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll(int pages)
    {
        return Ok(await _services.PeriodService.GetAll(pages));
    }
    [HttpGet("FilterByTime")]
    public async Task<IActionResult> FilterByTime(DateTime from  , DateTime to)
    {
        return Ok(await _services.PeriodService.GetAll(from, to));
    }
    [HttpGet("Search")]
    public async Task<IActionResult> Search(string criteria)
    {
        return Ok(await _services.PeriodService.Search(criteria));
    }

    [HttpGet("GetSelectList")]
    public async Task<IActionResult> GetSelectList()
    {
        return Ok(await _services.PeriodService.GetAllSelectList());
    }

    [HttpGet("New")]
    public async Task<IActionResult> New()
    {
        return Ok(await _services.PeriodService.New());
    }

    [HttpGet("Get/{id}")]
    public async Task<IActionResult> Get(int id)
    {
        return Ok(await _services.PeriodService.GetById(id));
    }


    [HttpPost("Create")]
    public async Task<IActionResult> Create(CreatePeriodDTO DTO)
    {
        var result = await _services.PeriodService.Create(DTO);

        if (result.IsSucceed)
            return Ok(result.Message);

        return BadRequest(result.Message);
    }

    [HttpPut("Edit")]
    public async Task<IActionResult> Edit(CreatePeriodDTO DTO)
    {
        var result = await _services.PeriodService.Edit(DTO);

        if (result.IsSucceed)
            return Ok(result.Message);

        return BadRequest(result.Message);
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _services.PeriodService.Delete(id);

        if (result.IsSucceed)
            return Ok(result.Message);

        return BadRequest(result.Message);
    }



}
