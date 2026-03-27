namespace Domain.DTO.Response;

public class AccountBalanceDTO
{
    public string AccountName { get; set; }
    public string Balance { get; set; }
    public int AccountId { get; set; }
    public bool IsRevExp { get; set; }
    public bool IsCredit { get; set; }

}
