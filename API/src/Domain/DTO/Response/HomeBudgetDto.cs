namespace Domain.DTO.Response;

public class HomeBudgetDto
{
   public IEnumerable<BudgetAccountDTO>? BudgetProgress { get; set;}
   public BudgetAccountDTO? CurrentCash { get; set; }
   public decimal OtherExpenses { get; set;}
   public decimal OverBudget { get; set;}
   public decimal AvailableFunds { get; set;}
   public decimal OtherExpensesTarget { get; set;}
   public decimal TotalRemains { get; set;}
   public decimal CurrentCashBudgetAfterRemian { get; set;}
}
