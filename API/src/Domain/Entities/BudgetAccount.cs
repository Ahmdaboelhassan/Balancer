using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Entities
{
    public class BudgetAccount
    {
        [Key]
        public int Id { get; set; }
        public int AccountId { get; set; }
        public decimal Budget { get; set; }
        public decimal BestBudget { get; set; }
        public decimal MaximumBudget { get; set; }
        public PaidMethods PaidMethodId { get; set; }
        public BudgetAccountTypes TypeId { get; set; }
        public string Color { get; set; }
        public string? Notes { get; set; }
        public string DisplayName { get; set; }

        [ForeignKey(nameof(AccountId))]
        public Account Account { get; set; }
    }
}
