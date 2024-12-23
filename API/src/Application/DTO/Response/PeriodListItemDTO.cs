namespace Application.DTO.Response;
public class PeriodListItemDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string From { get; set; }
    public string To { get; set; }
    public bool IncreasedBalance { get; set; }
    public decimal TotalAmount { get; set; }
    public int MaxPages { get; set; }
}
