namespace Domain.DTO.Response;

public class AccountBalanceDTO
{
    public string? AccountName { get; set; }
    public string? Balance { get; set; }
    public int AccountId { get; set; }
    public bool IsRevExp { get; set; }
    public bool IsCredit { get; set; }
    public string? AccountDescreption { get; set; }
    public LastAccountJournal? LastJournal { get; set; }

}

public class LastAccountJournal
{
    public int JournalId { get; set; }
    public byte Type { get; set; }
    public string? Name { get; set; }
    public decimal? Amount { get; set; }
    public string? DebitAccount { get; set; }
    public string? CreditAccount { get; set; }
    public DateTime JournalDate { get; set; }
}

