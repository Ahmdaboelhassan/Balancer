using Domain.DTO.Request;
using Domain.Entities;

namespace Application.Mappers
{
    public static class SettingMapper
    {
        public static SettingDTO ToDTO(this Settings setting)
        {
            if (setting == null)
                return null;

            return new SettingDTO
            {
                Id = setting.Id,
                DefaultCreditAccount = setting.DefaultCreditAccount,
                DefaultDebitAccount = setting.DefaultDebitAccount,
                DefaultPeriodDays = setting.DefaultPeriodDays,
                DefaultDayRate = setting.DefaultDayRate,
                ExpensesAccount = setting.ExpensesAccount,
                RevenueAccount = setting.RevenueAccount,
                AssetsAccount = setting.AssetsAccount,
                CurrentAssetsAccount = setting.CurrentAssetsAccount,
                FixedAssetsAccount = setting.FixedAssetsAccount,
                CurrentCashAccount = setting.CurrentCashAccount,
                LiabilitiesAccount = setting.LiabilitiesAccount,
                BanksAccount = setting.BanksAccount,
                DrawersAccount = setting.DrawersAccount,
                LevelOneDigits = setting.LevelOneDigits,
                LevelTwoDigits = setting.LevelTwoDigits,
                LevelThreeDigits = setting.LevelThreeDigits,
                LevelFourDigits = setting.LevelFourDigits,
                LevelFiveDigits = setting.LevelFiveDigits,
                MaxAccountLevel = setting.MaxAccountLevel
            };
        }


        public static Settings ToEntity(this SettingDTO settingDTO)
        {
            if (settingDTO == null)
                return null;

            return new Settings
            {
                Id = settingDTO.Id,
                DefaultCreditAccount = settingDTO.DefaultCreditAccount,
                DefaultDebitAccount = settingDTO.DefaultDebitAccount,
                DefaultPeriodDays = settingDTO.DefaultPeriodDays,
                DefaultDayRate = settingDTO.DefaultDayRate,
                ExpensesAccount = settingDTO.ExpensesAccount,
                RevenueAccount = settingDTO.RevenueAccount,
                AssetsAccount = settingDTO.AssetsAccount,
                CurrentAssetsAccount = settingDTO.CurrentAssetsAccount,
                FixedAssetsAccount = settingDTO.FixedAssetsAccount,
                CurrentCashAccount = settingDTO.CurrentCashAccount,
                LiabilitiesAccount = settingDTO.LiabilitiesAccount,
                BanksAccount = settingDTO.BanksAccount,
                DrawersAccount = settingDTO.DrawersAccount,
                LevelOneDigits = settingDTO.LevelOneDigits,
                LevelTwoDigits = settingDTO.LevelTwoDigits,
                LevelThreeDigits = settingDTO.LevelThreeDigits,
                LevelFourDigits = settingDTO.LevelFourDigits,
                LevelFiveDigits = settingDTO.LevelFiveDigits,
                MaxAccountLevel = settingDTO.MaxAccountLevel
            };
        }
    }
}
