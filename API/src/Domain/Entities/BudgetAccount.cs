using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class BudgetAccount
    {
        [Key]
        public int Id { get; set; }
        public int AccountId { get; set; }
        public decimal Budget { get; set; }
        public string Color { get; set; }
        public string DisplayName { get; set; }

        [ForeignKey(nameof(AccountId))]
        public Account Account { get; set; }
    }
}
