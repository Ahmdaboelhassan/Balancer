using Domain.DTO.Both;
using Domain.Entities;

namespace Application.Mappers
{
    public static class DashboardSettingsMapper
    {
        public static DashboardSettingsDTO ToDTO(this DashboardSettings dashboardSettings)
        {
            if (dashboardSettings == null)
                return null;

            return new DashboardSettingsDTO
            {
                Id = dashboardSettings.Id,
                Account1 = dashboardSettings.Account1,
                Account2 = dashboardSettings.Account2,
                Account3 = dashboardSettings.Account3,
                Account4 = dashboardSettings.Account4,
                OtherExpensesTarget = dashboardSettings.OtherExpensesTarget,
                AddOnExpensesTarget = dashboardSettings.AddOnExpensesTarget,
                ApplyOverBudgetToFunds = dashboardSettings.ApplyOverBudgetToFunds
            };
        }

        /// <summary>
        /// Converts DashboardSettingsDTO to DashboardSettings entity
        /// </summary>
        /// <param name="dashboardSettingsDTO">The DashboardSettingsDTO to convert</param>
        /// <returns>DashboardSettings entity object</returns>
        public static DashboardSettings ToEntity(this DashboardSettingsDTO dashboardSettingsDTO)
        {
            if (dashboardSettingsDTO == null)
                return null;

            return new DashboardSettings
            {
                Id = dashboardSettingsDTO.Id,
                Account1 = dashboardSettingsDTO.Account1,
                Account2 = dashboardSettingsDTO.Account2,
                Account3 = dashboardSettingsDTO.Account3,
                Account4 = dashboardSettingsDTO.Account4,
                OtherExpensesTarget = dashboardSettingsDTO.OtherExpensesTarget,
                AddOnExpensesTarget = dashboardSettingsDTO.AddOnExpensesTarget,
                ApplyOverBudgetToFunds = dashboardSettingsDTO.ApplyOverBudgetToFunds
            };
        }

        /// <summary>
        /// Converts a collection of DashboardSettings entities to DashboardSettingsDTO collection
        /// </summary>
        /// <param name="dashboardSettings">Collection of DashboardSettings entities</param>
        /// <returns>Collection of DashboardSettingsDTO objects</returns>
        public static IEnumerable<DashboardSettingsDTO> ToDTO(this IEnumerable<DashboardSettings> dashboardSettings)
        {
            return dashboardSettings?.Select(ds => ds.ToDTO()) ?? new List<DashboardSettingsDTO>();
        }

        /// <summary>
        /// Converts a collection of DashboardSettingsDTO to DashboardSettings entity collection
        /// </summary>
        /// <param name="dashboardSettingsDTOs">Collection of DashboardSettingsDTO objects</param>
        /// <returns>Collection of DashboardSettings entities</returns>
        public static IEnumerable<DashboardSettings> ToEntity(this IEnumerable<DashboardSettingsDTO> dashboardSettingsDTOs)
        {
            return dashboardSettingsDTOs?.Select(dto => dto.ToEntity()) ?? new List<DashboardSettings>();
        }
    }
}
