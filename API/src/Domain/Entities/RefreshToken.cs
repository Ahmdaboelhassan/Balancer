using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class RefreshToken
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Token { get; set; }
    [Required]
    public int UserId { get; set; }
    [Required]
    public DateTime ExpireOn { get; set; }
    [Required]
    public bool IsRevoked { get; set; }

    [Required]
    [ForeignKey(nameof(UserId))]
    public User User { get; set; }
}
