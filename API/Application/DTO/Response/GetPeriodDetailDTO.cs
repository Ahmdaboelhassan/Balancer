namespace Application.DTO.Response;
public class GetPeriodDetailDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateOnly From { get; set; }
    public DateOnly To { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
    public int DaysCount { get; set; }
    public string? Notes { get; set; }
    public decimal TotalAmount { get; set; }
    public ICollection<JournalListItemDTO> Journals { get; set; }
}
