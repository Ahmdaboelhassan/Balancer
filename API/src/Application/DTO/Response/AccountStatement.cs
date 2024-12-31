namespace Application.DTO.Response;
public class AccountStatement
{
    public string AccountType { get; set; }
    public string AccountName { get; set; }
    public string From { get; set; }
    public string To { get; set; }
    public string Amount { get; set; }
    public IEnumerable<AccountStatementDetail> Details { get; set; }
}
