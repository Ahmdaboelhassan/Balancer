using Domain.Entities;

namespace Domain.DTO.Response;

public class GetHomeDTO
{
    public IEnumerable<AccountBalanceDTO> AccountsSummary { get; set; }
    public IEnumerable<decimal> LastPeriods { get; set; }
    public IEnumerable<decimal> CurrentAndLastMonthExpenses { get; set; }
    public IEnumerable<decimal> CurrentYearExpenses { get; set; }
    public IEnumerable<decimal> CurrentYearRevenues{ get; set; }
    public IEnumerable<BudgetAccountDTO> BudgetProgress { get; set; }
    public decimal AvailableFunds { get; set; }
    public decimal DayRate { get; set; }
    public decimal PeriodDays { get; set; }
}
