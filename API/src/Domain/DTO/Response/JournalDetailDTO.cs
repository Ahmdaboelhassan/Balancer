namespace Domain.DTO.Response;
public class JournalDetailDTO
{
    public string AccountName { get; set; }
    public string CostCenterName { get; set; }
    public decimal Credit { get; set; }
    public decimal Debit { get; set; }
    public string Notes { get; set; }
    public string PeriodName { get; set; }
    public int PeriodId { get; set; }
    public int JournalId { get; set; }
    public DateTime? JournalDate { get; set; }

}
