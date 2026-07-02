
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class JournalDetailCostCenter
{
    [Key]
    public int Id { get; set; }
    public int JournalDetailId { get; set; }
    public int CostCenterId { get; set; }
    public bool IsDeleted { get; set; }

    public JournalDetail JournalDetail { get; set; }
    public CostCenter CostCenter { get; set; }
}
