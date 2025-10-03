namespace Domain.DTO.Response;
public class AccountStatementDetail
{
   public string Detail { get; set; }
   public string notes { get; set; }
   public decimal Debit { get; set; }
   public decimal Credit { get; set; }
   public decimal Balance { get; set; }
   public string CostCenter { get; set; }
   public int JournalId { get; set; }
   public int PeriodId { get; set; }
   public string? Date { get; set; }
   public string? Description { get; set; }
}
