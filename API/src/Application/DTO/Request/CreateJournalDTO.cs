using System.ComponentModel.DataAnnotations;

namespace Application.DTO.Request;
public class CreateJournalDTO
{
    public int Id { get; set; }
    [Required]
    public string Detail { get; set; }
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public int PeriodId { get; set; }
    public int? CostCenterId { get; set; }
    [Required]
    public int CreditAccountId { get; set; }
    [Required]
    public int DebitAccountId { get; set; }
}
