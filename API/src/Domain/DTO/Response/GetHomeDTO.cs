namespace Domain.DTO.Response;

public class GetHomeDTO
{
    public IEnumerable<AccountBalanceDTO> AccountsSummary { get; set; }
    public IEnumerable<decimal> LastPeriods { get; set; }
    public IEnumerable<decimal> CurrentAndLastMonthExpenses { get; set; }
    public IEnumerable<decimal> CurrentYearExpenses { get; set; }
    public IEnumerable<decimal> CurrentYearRevenues{ get; set; }

    public decimal PeriodExpensesTarget { get; set; }
    public decimal OtherExpensesTarget { get; set; }
    public decimal GamieaLiabilitiesTarget { get; set; }
    public decimal MonthlySavingsTarget { get; set; }
    public decimal PeriodExpensesAmount { get; set; }
    public decimal OtherExpensesAmount { get; set; }
    public decimal GamieaLiabilitiesAmount { get; set; }
    public decimal MonthlySavingsAmount { get; set; }
    public decimal DayRate { get; set; }
    public decimal AvailableFunds { get; set; }
    public decimal PeriodDays { get; set; }
}
