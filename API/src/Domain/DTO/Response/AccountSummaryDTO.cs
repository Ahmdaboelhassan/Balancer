namespace Domain.DTO.Response;
public class AccountSummaryDTO
{
    public string AccountName { get; set; }
    public string AccountNumber { get; set; }
    public int AccountId { get; set; }
    public decimal Balance { get; set; }
    public decimal Credit { get; set; }
    public decimal Debit { get; set; }
}
