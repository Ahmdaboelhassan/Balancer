using Application.DTO.Request;
using Application.IServices;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("[controller]")]
[ApiController]
public class JournalController : ControllerBase
{
    private readonly IServiceContext _serviceContext;
    public JournalController(IServiceContext serviceContext)
    {
        _serviceContext = serviceContext;
    }

    [HttpGet("New/{periodId}")]
    public async Task<IActionResult> New(int periodId)
    {
        return Ok(await _serviceContext.JournalService.New(periodId));

    }

    [HttpGet("Get/{id}")]
    public async Task<IActionResult> Get(int id)
    {
        return Ok(await _serviceContext.JournalService.Get(id));
    }
    [HttpGet("Search")]
    public async Task<IActionResult> Search(string criteria)
    {
        return Ok(await _serviceContext.JournalService.Search(criteria));
    }
    [HttpGet("GetAll/{page}")]
    public async Task<IActionResult> GetAll(int page)
    {
        return Ok(await _serviceContext.JournalService.GetAll(page));
    }
    [HttpGet("FilterByTime")]
    public async Task<IActionResult> FilterByTime(DateTime from , DateTime to)
    {
        return Ok(await _serviceContext.JournalService.GetAll(from , to));
    }
    [HttpGet("GetJournals/{periodId}")]
    public async Task<IActionResult> GetJournals(int periodId)
    {
        return Ok(await _serviceContext.JournalService.GetPeriodJournals(periodId));
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create(CreateJournalDTO model)
    {
        var result = await _serviceContext.JournalService.Create(model); 
        if (result.IsSucceed)
            return Ok (result.Message);

        return BadRequest(result.Message);
    }

    [HttpPut("Edit")]
    public async Task<IActionResult> Edit(CreateJournalDTO model)
    {
        var result = await _serviceContext.JournalService.Edit(model);
        if (result.IsSucceed)
            return Ok(result.Message);

        return BadRequest(result.Message);
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _serviceContext.JournalService.Delete(id);
        if (result.IsSucceed)
            return Ok(result.Message);

        return BadRequest(result.Message);
    }


}
