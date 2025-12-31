using Domain.Entities;

namespace Domain.DTO.Response;

public class GetHomeDTO
{
    public IEnumerable<AccountBalanceDTO> AccountsSummary { get; set; }
    public IEnumerable<decimal> LastPeriods { get; set; }
    public IEnumerable<decimal> JournalsTypesSummary { get; set; }
    public IEnumerable<decimal> Expenses { get; set; }
    public IEnumerable<decimal> Revenues{ get; set; }
    public IEnumerable<string> MonthsNames{ get; set; }
    public IEnumerable<BudgetAccountDTO> BudgetProgress { get; set; }
    public decimal AvailableFunds { get; set; }
    public decimal OtherExpensesTarget { get; set; }
    public decimal DayRate { get; set; }
    public decimal PeriodDays { get; set; }
    public decimal OtherExpenses { get; set; }
}
