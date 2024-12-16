namespace Application.DTO.Response;
public class JournalListItemDTO
{
    public int Code { get; set; }
    public string Detail { get; set; }
    public DateTime CreatedAt { get; set; }
    public byte Type { get; set; }
    public decimal Amount { get; set; }
    public string? Notes { get; set; }
}
