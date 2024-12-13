using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models;

public class Account
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string Name { get; set; }

    [Required]
    public string Number { get; set; }
    [Required]
    public int Level { get; set; }
    public string? Description { get; set; }
    public int? ParentId { get; set; }
    public bool IsParent { get; set; }
    public bool IsDeleted { get; set; }
    [ForeignKey(nameof(ParentId))]
    public Account? Parent { get; set; }
}
