using Application.DTO.Request;
using Application.DTO.Response;
using Application.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class JournalController : ControllerBase
{
    private readonly IServiceContext _serviceContext;
    public JournalController(IServiceContext serviceContext)
    {
        _serviceContext = serviceContext;
    }

    [HttpPost("CreateJournal")]
    public async Task<IActionResult> CreateJournal(CreateJournalDTO model)
    {
        var result = await _serviceContext.JournalService.CreateJournal(model); 
        if (result.IsSucceed)
            return Ok (result.Message);

        return BadRequest(result.Message);
    }

    [HttpPost("EditJournal")]
    public async Task<IActionResult> EditJournal(CreateJournalDTO model)
    {
        var result = await _serviceContext.JournalService.EditJournal(model);
        if (result.IsSucceed)
            return Ok(result.Message);

        return BadRequest(result.Message);
    }


}
