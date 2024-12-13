namespace Domain.Models;

public class Settings
{
    public int Id { get; set; }
    public int DefaultCreditAccount { get; set; }
    public int DefaultDebitAccount { get; set; }
    public int ExpensesAccount { get; set; }
    public int RevenueAccount { get; set; }
    public int AssetsAccount { get; set; }
    public int LiabilitiesAccount { get; set; }
    public int LevelOneDigits { get; set; }
    public int LevelTwoDigits { get; set; }
    public int LevelThreeDigits { get; set; }
    public int LevelFourDigits { get; set; }
}
