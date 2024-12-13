
using System.ComponentModel.DataAnnotations;

namespace Application.DTO.Request;
public class AccountDTO
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    public string? Description { get; set; }
    public int? ParentId { get; set; }
    public string? AccountNumber { get; set; }
    public string? ParentNumber { get; set; }
    public string? ParentName { get; set; }
    public bool IsParent { get; set; }
}
