using System.ComponentModel.DataAnnotations;

namespace Domain.DTO.Both
{
    public class SettingDTO
    {
        public int Id { get; set; }
        [Required]
        public int? DefaultCreditAccount { get; set; }
        [Required]
        public int? DefaultDebitAccount { get; set; }
        public int? DefaultPeriodDays { get; set; } = 7;
        public decimal DefaultDayRate { get; set; }
        public int? NotBudgetCostCenter { get; set; }

        [Required]
        public int? ExpensesAccount { get; set; }
        [Required]
        public int? RevenueAccount { get; set; }
        [Required]
        public int? AssetsAccount { get; set; }
        [Required]
        public int? CurrentAssetsAccount { get; set; }
        [Required]
        public int? FixedAssetsAccount { get; set; }
        [Required]
        public int? CurrentCashAccount { get; set; }
        [Required]
        public int? LiabilitiesAccount { get; set; }
        [Required]
        public int? BanksAccount { get; set; }
        [Required]
        public int? DrawersAccount { get; set; }
        [Required]
        public int LevelOneDigits { get; set; }
        [Required]
        public int LevelTwoDigits { get; set; }
        [Required]
        public int LevelThreeDigits { get; set; }
        [Required]
        public int LevelFourDigits { get; set; }
        [Required]
        public int LevelFiveDigits { get; set; }
        [Required]
        public int MaxAccountLevel { get; set; }
    }
}
