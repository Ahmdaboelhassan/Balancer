using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class Journal
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int Code { get; set; }
        [Required]
        public string Detail { get; set; }
        [Required]
        public string Notes { get; set; }
        public string? Description { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public byte Type { get; set; }
        public decimal Amount { get; set; }

        [Required]
        public int PeriodId { get; set; }
  
        [ForeignKey(nameof(PeriodId))]
        public Period Period { get; set; }
        public ICollection<JournalDetail> JournalDetails { get; set; }
    }
}
