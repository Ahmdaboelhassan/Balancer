using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class JournalController : ControllerBase
{
    private readonly IServiceContext _serviceContext;
    public JournalController(IServiceContext serviceContext)
    {
        _serviceContext = serviceContext;
    }

    [HttpGet("New")]
    public async Task<IActionResult> New(int? periodId)
    {   
        var result = await _serviceContext.JournalService.New(periodId);
        if (result is null)
            return BadRequest(new { message = "There Are No Period To Assign" });
            
        return Ok(result);
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
    [HttpGet("AdvancedSearch")]
    public async Task<IActionResult> JournalsFilter([FromQuery]JournalAdvancedSearchDTO DTO)
    {
        return Ok(await _serviceContext.JournalService.AdvancedSearch(DTO));
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

    [HttpGet("GetNextJournal/{date}")]
    public async Task<IActionResult> GetNextJournal(string date)
    {
        return Ok(await _serviceContext.JournalService.GetNextJournal(date));
    }

    [HttpGet("GetPrevJournal/{date}")]
    public async Task<IActionResult> GetPrevJournal(string date)
    {
        return Ok(await _serviceContext.JournalService.GetPrevJournal(date));
    }

    
    [HttpPost("AdjustBudget")]
    public async Task<IActionResult> AdjustBudget(AdjustBudgetRequest dto)
    {
        var result = await _serviceContext.JournalService.AdjustBudget(dto); 
        if (result.IsSucceed)
            return Ok (result);

        return BadRequest(result);
    }
    
    [HttpPost("Create")]
    public async Task<IActionResult> Create(CreateJournalDTO model)
    {
        var result = await _serviceContext.JournalService.Create(model); 
        if (result.IsSucceed)
            return Ok (result);

        return BadRequest(result);
    }

    [HttpPut("Edit")]
    public async Task<IActionResult> Edit(CreateJournalDTO model)
    {
        var result = await _serviceContext.JournalService.Edit(model);
        if (result.IsSucceed)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _serviceContext.JournalService.Delete(id);
        if (result.IsSucceed)
            return Ok(result);

        return BadRequest(result);
    }


}
