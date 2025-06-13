namespace Domain.Entities;
public class DashboardSettings
{
    public int Id { get; set; }
    public int Account1 { get; set; }
    public int Account2 { get; set; }
    public int Account3 { get; set; }
    public int Account4 { get; set; }
    public decimal OtherExpensesTarget { get; set; }
    public decimal AddOnExpensesTarget { get; set; }
    public bool ApplyOverBudgetToFunds { get; set; }
}
