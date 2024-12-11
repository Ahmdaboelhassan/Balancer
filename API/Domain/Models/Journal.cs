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
        
        [Required]
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        [Required]
        public int PeriodId { get; set; }
  
        [ForeignKey(nameof(PeriodId))]
        public Period Period { get; set; }

        public ICollection<JournalDetail> JournalDetails { get; set; }
    }
}
