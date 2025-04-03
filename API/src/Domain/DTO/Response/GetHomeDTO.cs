namespace Domain.DTO.Response;

public class GetHomeDTO
{
    public IEnumerable<AccountBalanceDTO> AccountsSummary { get; set; }
    public IEnumerable<decimal> CurrentAndLastMonthExpenses { get; set; }
    public IEnumerable<decimal> CurrentYearExpenses { get; set; }
    public IEnumerable<decimal> CurrentYearRevenues{ get; set; }
}
