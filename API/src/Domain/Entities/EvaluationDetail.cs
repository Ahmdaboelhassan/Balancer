using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class EvaluationDetail
    {
        [Key]
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int EvaluationId { get; set; }
        public decimal Amount { get; set; }
        public bool IsDeleted { get; set; }
        
        [ForeignKey(nameof(AccountId))]
        public Account Account { get; set; }

        [ForeignKey(nameof(EvaluationId))]
        public Evaluation Evaluation { get; set; }
    }
}
