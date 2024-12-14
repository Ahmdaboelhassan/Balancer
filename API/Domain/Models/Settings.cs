namespace Domain.Models;

public class Settings
{
    public int Id { get; set; }
    public int DefaultCreditAccount { get; set; }
    public int DefaultDebitAccount { get; set; }
    public string? ExpensesAccountNumber { get; set; }
    public string? RevenueAccountNumber { get; set; }
    public string? AssetsAccountNumber { get; set; }
    public string? LiabilitiesAccountNumber { get; set; }
    public int LevelOneDigits { get; set; }
    public int LevelTwoDigits { get; set; }
    public int LevelThreeDigits { get; set; }
    public int LevelFourDigits { get; set; }
    public int MaxAccountLevel { get; set; }
}
