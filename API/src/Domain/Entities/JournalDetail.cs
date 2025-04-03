using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;
public class JournalDetail
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int JournalId { get; set; }
    [Required]
    public int AccountId { get; set; }
    public int? CostCenterId { get; set; }
    public string? Notes { get; set; }
    public decimal Credit { get; set; }
    public decimal Debit { get; set; }
    public bool IsDeleted { get; set; }

    [ForeignKey(nameof(AccountId))]
    public Account Account { get; set; }

    [ForeignKey(nameof(JournalId))]
    public Journal Journal { get; set; }

    [ForeignKey(nameof(CostCenterId))]
    public CostCenter? CostCenter { get; set; }

}
