namespace Application.DTO.Response;
public class JournalListItemDTO
{
    public int Code { get; set; }
    public string Detail { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IncreasedBalance { get; set; }
}
