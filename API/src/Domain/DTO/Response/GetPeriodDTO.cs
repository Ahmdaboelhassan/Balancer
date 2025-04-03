using Domain.DTO.Request;

namespace Domain.DTO.Response;
public class GetPeriodDTO : CreatePeriodDTO
{
    public string Name { get; set; }
    public string CreatedAt { get; set; }
    public string? LastUpdatedAt { get; set; }
    public decimal TotalAmount { get; set; }
}
