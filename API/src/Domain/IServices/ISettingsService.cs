using Domain.DTO.Both;
using Domain.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IServices
{
   public interface ISettingsService
    {
      Task<SettingDTO> GetSettings();
      Task<ConfirmationResponse> EditSettings(SettingDTO settings);
      Task<DashboardSettingsDTO> GetDashboardSettings();
      Task<ConfirmationResponse> EditDashboardSettings(DashboardSettingsDTO settings);
      Task<IEnumerable<BudgetAccountSettingsDTO>> GetBudgetAccountSettings();
      Task<ConfirmationResponse> EditBudgetAccountSettings(IEnumerable<BudgetAccountSettingsDTO> settings);
    }
}
