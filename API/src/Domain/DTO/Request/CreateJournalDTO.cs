using Domain.DTO.Response;
using System.ComponentModel.DataAnnotations;

namespace Domain.DTO.Request;
public class CreateJournalDTO
{
    public int Id { get; set; }
    [Required]
    public string Detail { get; set; }
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public int PeriodId { get; set; }
    public DateTime CreatedAt { get; set; } 
    public List<int> CostCentersIds { get; set; } = new List<int>();

    [Required]
    public int CreditAccountId { get; set; }
    [Required]
    public int DebitAccountId { get; set; }
}
