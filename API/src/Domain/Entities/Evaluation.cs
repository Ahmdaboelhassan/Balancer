using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Evaluation
    {
        [Key]
        public int Id { get; set; }
        public required DateTime From { get; set; }
        public required DateTime To { get; set; }
        public string? Name { get; set; }
        public bool IsDeleted { get; set; }
        public decimal Profit { get; set; }
        public decimal Income { get; set; }
        public int userId { get; set; }
        public string? Note { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<EvaluationDetail> EvaluationDetails { get; set; }

    }
}
