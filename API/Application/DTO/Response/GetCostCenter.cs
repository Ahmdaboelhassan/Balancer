using Application.DTO.Request;

namespace Application.DTO.Response;
public class GetCostCenter : CreateCostCenter 
{ 
    public DateTime CreatedAt { get; set; }
}
