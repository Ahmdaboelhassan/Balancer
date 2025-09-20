namespace Domain.Entities;

public class Settings
{
    public int Id { get; set; }
    public int? DefaultCreditAccount { get; set; }
    public int? DefaultDebitAccount { get; set; }
    public int? DefaultPeriodDays { get; set; }
    public decimal DefaultDayRate { get; set; }
    
    public int? ExpensesAccount { get; set; }
    public int? RevenueAccount { get; set; }
    public int? AssetsAccount { get; set; }
    public int? CurrentAssetsAccount { get; set; }
    public int? FixedAssetsAccount { get; set; }
    public int? CurrentCashAccount { get; set; }
    public int? LiabilitiesAccount { get; set; }
    public int? BanksAccount { get; set; }
    public int? DrawersAccount { get; set; }
    public int? NotBudgetCostCenter { get; set; }
    public int LevelOneDigits { get; set; }
    public int LevelTwoDigits { get; set; }
    public int LevelThreeDigits { get; set; }
    public int LevelFourDigits { get; set; }
    public int LevelFiveDigits { get; set; }
    public int MaxAccountLevel { get; set; }
}
