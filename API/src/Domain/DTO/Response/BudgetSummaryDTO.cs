namespace Domain.DTO.Response;
public class BudgetSummaryDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal? Budget { get; set; }
    public decimal Spent { get; set; }
    public decimal? Remains { get; set; }
    public decimal? SpentPercentage { get; set; }
    public decimal? RemainPercentage { get; set; }
    public bool IsOtherExpenses { get; set; }
    public bool IsSavingTarget { get; set; }
}


public class BudgetSummaryReportDTO
{
    public IEnumerable<BudgetSummaryDTO> Periods { get; set; } = new List<BudgetSummaryDTO>();
    public IEnumerable<BudgetSummaryDTO> Accounts { get; set; } = new List<BudgetSummaryDTO>();
    public IEnumerable<BudgetSummaryDTO> Savings { get; set; } = new List<BudgetSummaryDTO>();
}