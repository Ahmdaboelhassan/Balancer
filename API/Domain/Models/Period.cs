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
        public DateOnly From { get; set; }
        public DateOnly To { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        [Required]
        public int DaysCount { get; set; }
        public string? Notes { get; set; }

        public ICollection<Journal> Journals { get; set; }
    }
}
