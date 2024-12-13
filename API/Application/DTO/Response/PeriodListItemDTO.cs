namespace Application.DTO.Response;
public class PeriodListItemDTO
{
    public string Name { get; set; }
    public DateOnly From { get; set; }
    public DateOnly To { get; set; }
    public bool IncreasedBalance { get; set; }
}
