using Application.Services;
using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class EvaluationController : ControllerBase
    {
        public IEvaluationService _evaluationService { get; set; }

        public EvaluationController(IEvaluationService evaluationService)
        {
            _evaluationService = evaluationService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(DateTime from , DateTime to)
        {
            return Ok(await _evaluationService.GetAll(from , to));
        }

        [HttpGet("New")]
        public async Task<IActionResult> New()
        {
            return Ok(await _evaluationService.New());
        }

        [HttpGet("Get/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            return Ok(await _evaluationService.GetById(id));
        }

        [HttpPost("CalculateDetailsBalances")]
        public async Task<IActionResult> CalculateDetailsBalances(EvaluationDTO DTO)
        {
            var result = await _evaluationService.CalculateDetailsBalances(DTO);

            if (!result.IsSucceed)
                return BadRequest(result);

             return Ok(result);

        }


        [HttpPost("Create")]
        public async Task<IActionResult> Create(EvaluationDTO DTO)
        {
            var claim = (ClaimsIdentity)User.Identity;
            var userId = int.Parse(claim.FindFirst(ClaimTypes.NameIdentifier).Value);

            var result = await _evaluationService.Create(DTO, userId);

            if (result.IsSucceed)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpPut("Edit")]
        public async Task<IActionResult> Edit(EvaluationDTO DTO)
        {
            var result = await _evaluationService.Edit(DTO);

            if (result.IsSucceed)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _evaluationService.Delete(id);

            if (result.IsSucceed)
                return Ok(result);

            return BadRequest(result);
        }

    }
}
