namespace Domain.DTO.Response;
public class PeriodJournals
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string From { get; set; }
    public string To { get; set; }
    public IEnumerable<JournalListItemDTO> Journals { get; set; }
}
