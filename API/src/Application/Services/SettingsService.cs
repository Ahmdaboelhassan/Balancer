using Application.Mappers;
using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;
using Domain.IRepository;
using Domain.IServices;

namespace Application.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SettingsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SettingDTO> GetSettings()
        {

            var settings = await _unitOfWork.Settings.GetFirst();

            if (settings == null)
                return new SettingDTO();

            return settings.ToDTO();
        }

        public async Task<ConfirmationResponse> EditSettings(SettingDTO settings)
        {
            var existingSettings = await _unitOfWork.Settings.GetFirst();

            if (existingSettings == null)
            {
                await _unitOfWork.Settings.AddAsync(settings.ToEntity());
                await _unitOfWork.SaveChangesAync();
                return new  ConfirmationResponse(){ IsSucceed = true, Message = "Settings created successfully."};
            }
       
            // Update existing settings

            existingSettings.DefaultCreditAccount = settings.DefaultCreditAccount;
            existingSettings.DefaultDebitAccount = settings.DefaultDebitAccount;
            existingSettings.DefaultPeriodDays = settings.DefaultPeriodDays;
            existingSettings.DefaultDayRate = settings.DefaultDayRate;
            existingSettings.ExpensesAccount = settings.ExpensesAccount;
            existingSettings.RevenueAccount = settings.RevenueAccount;
            existingSettings.AssetsAccount = settings.AssetsAccount;
            existingSettings.CurrentAssetsAccount = settings.CurrentAssetsAccount;
            existingSettings.FixedAssetsAccount = settings.FixedAssetsAccount;
            existingSettings.CurrentCashAccount = settings.CurrentCashAccount;
            existingSettings.LiabilitiesAccount = settings.LiabilitiesAccount;
            existingSettings.BanksAccount = settings.BanksAccount;
            existingSettings.DrawersAccount = settings.DrawersAccount;
            existingSettings.LevelOneDigits = settings.LevelOneDigits;
            existingSettings.LevelTwoDigits = settings.LevelTwoDigits;
            existingSettings.LevelThreeDigits = settings.LevelThreeDigits;
            existingSettings.LevelFourDigits = settings.LevelFourDigits;
            existingSettings.LevelFiveDigits = settings.LevelFiveDigits;
            existingSettings.MaxAccountLevel = settings.MaxAccountLevel;

            _unitOfWork.Settings.Update(existingSettings);
            await _unitOfWork.SaveChangesAync();

            return new ConfirmationResponse() { IsSucceed = true, Message = "Settings updated successfully." };
        }


        public async Task<DashboardSettingsDTO> GetDashboardSettings()
        {
            var settings = await _unitOfWork.DashboardSettings.GetFirst();

            if (settings == null)
                return new DashboardSettingsDTO();

            return settings.ToDTO();
        }

        public async Task<ConfirmationResponse> EditDashboardSettings(DashboardSettingsDTO settings)
        {
            var existedSettings = await _unitOfWork.DashboardSettings.GetFirst();

            if (existedSettings == null)
            {
                await _unitOfWork.DashboardSettings.AddAsync(settings.ToEntity());
                await _unitOfWork.SaveChangesAync();
                return new ConfirmationResponse() { IsSucceed = true, Message = "Dashboard Settings created successfully." };
            }

            existedSettings.ApplyOverBudgetToFunds = settings.ApplyOverBudgetToFunds;
            existedSettings.OtherExpensesTarget = settings.OtherExpensesTarget;
            existedSettings.Account1 = settings.Account1;
            existedSettings.Account2 = settings.Account2;
            existedSettings.Account3 = settings.Account3;
            existedSettings.Account4 = settings.Account4;
            _unitOfWork.DashboardSettings.Update(existedSettings);
            await _unitOfWork.SaveChangesAync();

            return new ConfirmationResponse() { IsSucceed = true, Message = "Dashboard Settings updated successfully." };
        }


        public async Task<IEnumerable<BudgetAccountSettingsDTO>> GetBudgetAccountSettings()
        {
            var settings = await _unitOfWork.BudgetAccounts.GetAll();

            if (settings == null)
                return Enumerable.Empty<BudgetAccountSettingsDTO>();

            return settings.ToDTO();
        }

        public async Task<ConfirmationResponse> EditBudgetAccountSettings(IEnumerable<BudgetAccountSettingsDTO> settings)
        {
            var existedSettings = await _unitOfWork.BudgetAccounts.GetAll();

            if (existedSettings.Any())
                _unitOfWork.BudgetAccounts.DeleteRange(existedSettings);

            if (!settings.Any())
                return new ConfirmationResponse() { IsSucceed = false, Message = "No Budget Account Settings provided to update." };
            
            var newBudgetAccounts = settings.ToEntity();

            await _unitOfWork.BudgetAccounts.AddRange(newBudgetAccounts);
            await _unitOfWork.SaveChangesAync();
            
            return new ConfirmationResponse() { IsSucceed = true, Message = "Budget Account Settings created successfully." };

        }
    }
}
