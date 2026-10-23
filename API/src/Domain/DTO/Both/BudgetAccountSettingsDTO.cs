using Domain.Enums;

namespace Domain.DTO.Both
{
    public class BudgetAccountSettingsDTO
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public decimal Budget { get; set; }
        public decimal BestBudget { get; set; }
        public decimal MaximumBudget { get; set; }
        public PaidMethods PaidMethodId { get; set; }
        public BudgetAccountTypes TypeId { get; set; }
        public string Color { get; set; }
        public string DisplayName { get; set; }
        public string? Notes { get; set; }
    }
}
