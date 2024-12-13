
namespace Application.DTO.Response;
public record ConfirmationResponse 
{
    public bool IsSucceed { get; set; }
    public string Message { get; set; }
}
