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
    public DateTime CreatedAt { get; set; } // auto Generate
    public int? CostCenterId { get; set; }
    [Required]
    public int CreditAccountId { get; set; }
    [Required]
    public int DebitAccountId { get; set; }
}
