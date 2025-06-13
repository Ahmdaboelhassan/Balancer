using Domain.DTO.Both;
using Domain.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingsService _settingsService;

        public SettingsController(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        [HttpGet("GetSettings")]
        public async Task<IActionResult> GetSettings()
        {
            return Ok(await _settingsService.GetSettings());
        }

        [HttpPost("EditSettings")]
        public async Task<IActionResult> EditSettings(SettingDTO model)
        {
            var result = await _settingsService.EditSettings(model);
            if (result.IsSucceed)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpGet("GetDashboardSettings")]
        public async Task<IActionResult> GetDashboardSettings()
        {
            return Ok(await _settingsService.GetDashboardSettings());
        }

        [HttpPost("EditDashboardSettings")]
        public async Task<IActionResult> EditDashboardSettings(DashboardSettingsDTO model)
        {
            var result = await _settingsService.EditDashboardSettings(model);
            if (result.IsSucceed)
                return Ok(result);

            return BadRequest(result);
        }


        [HttpGet("GetBudgetAccountSettings")]
        public async Task<IActionResult> GetBudgetAccountSettings()
        {
            return Ok(await _settingsService.GetBudgetAccountSettings());
        }

        [HttpPost("EditBudgetAccountSettings")]
        public async Task<IActionResult> EditBudgetAccountSettings(IEnumerable<BudgetAccountSettingsDTO> model)
        {
            var result = await _settingsService.EditBudgetAccountSettings(model);
            if (result.IsSucceed)
                return Ok(result);

            return BadRequest(result);
        }


    }
}
