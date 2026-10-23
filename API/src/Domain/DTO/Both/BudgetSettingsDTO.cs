namespace Domain.DTO.Both
{
    public class BudgetSettingsDTO
    {
        public decimal OtherExpensesTarget { get; set; }
        public decimal AddOnExpensesTarget { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalIncome { get; set; }
        public int? SavingTargetAccountId { get; set; }
        public string? SavingTargetDisplayName { get; set; }
        public bool ApplyOverBudgetToFunds { get; set; }
        public IEnumerable<BudgetAccountSettingsDTO> BudgetAccounts { get; set; } = new List<BudgetAccountSettingsDTO>();
    }
}
