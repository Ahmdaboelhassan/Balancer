using Domain.DTO.Request;

namespace Domain.DTO.Response;
public class GetCostCenter : CreateCostCenter 
{ 
    public string CreatedAt { get; set; }
}
