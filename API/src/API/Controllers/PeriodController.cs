using Domain.DTO.Request;
using Domain.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[Route("[controller]")]
[ApiController]
[Authorize]
public class PeriodController : ControllerBase
{
    public readonly IPeriodService _periodService;

    public PeriodController(IPeriodService periodService)
    {
        _periodService = periodService;
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll(int pages)
    {
        return Ok(await _periodService.GetAll(pages));
    }
    [HttpGet("FilterByTime")]
    public async Task<IActionResult> FilterByTime(DateTime from  , DateTime to)
    {
        return Ok(await _periodService.GetAll(from, to));
    }
    [HttpGet("Search")]
    public async Task<IActionResult> Search(string criteria)
    {
        return Ok(await _periodService.Search(criteria));
    }

    [HttpGet("GetSelectList")]
    public async Task<IActionResult> GetSelectList()
    {
        return Ok(await _periodService.GetAllSelectList());
    }

    [HttpGet("New")]
    public async Task<IActionResult> New()
    {
        return Ok(await _periodService.New());
    }

    [HttpGet("Get/{id}")]
    public async Task<IActionResult> Get(int id)
    {
        return Ok(await _periodService.GetById(id));
    }


    [HttpPost("Create")]
    public async Task<IActionResult> Create(CreatePeriodDTO DTO)
    {
        var result = await _periodService.Create(DTO);

        if (result.IsSucceed)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpPut("Edit")]
    public async Task<IActionResult> Edit(CreatePeriodDTO DTO)
    {
        var result = await _periodService.Edit(DTO);

        if (result.IsSucceed)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _periodService.Delete(id);

        if (result.IsSucceed)
            return Ok(result);

        return BadRequest(result);
    }



}
