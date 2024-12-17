using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Period
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }

        [Required]
        public int DaysCount { get; set; }
        public string? Notes { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<Journal> Journals { get; set; }
    }
}
