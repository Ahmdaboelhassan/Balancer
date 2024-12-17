using Application.DTO.Request;

namespace Application.DTO.Response;
public class GetPeriodDTO : CreatePeriodDTO
{
    public string Name { get; set; }
    public DateTime To { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
    public decimal TotalAmount { get; set; }
    public ICollection<JournalListItemDTO> Journals { get; set; }
}
