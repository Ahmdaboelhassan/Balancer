using Domain.DTO.Request;

namespace Domain.DTO.Response;
public class GetCostCenter : CreateCostCenter 
{ 
    public DateTime CreatedAt { get; set; }
}
