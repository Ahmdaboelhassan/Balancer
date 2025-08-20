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
        public ICollection<EvaluationDetail> EvaluationDetails { get; set; }

    }
}
