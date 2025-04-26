namespace Domain.Entities;
public class DashboardSettings
{
    public int Id { get; set; }
    public int Account1 { get; set; }
    public int Account2 { get; set; }
    public int Account3 { get; set; }
    public int Account4 { get; set; }
    public int CurrentCashAccount { get; set; }
    public int GamieaLiabilitiesAccount { get; set; }
    public int PeriodsExpensesAccount { get; set; }

    public decimal PeriodExpensesTarget { get; set; }
    public decimal OtherExpensesTarget { get; set; }
    public decimal GamieaLiabilitiesTarget { get; set; }
    public decimal MonthlySavingsTarget { get; set; }
    public decimal DayRate { get; set; }
}
