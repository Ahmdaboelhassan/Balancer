using System.ComponentModel.DataAnnotations;

namespace Domain.Models;
public class CostCenter
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
