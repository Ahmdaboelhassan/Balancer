namespace Domain.DTO.Response;

public class RefreshTokenDTO
{
    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpireOn { get; set; }
}
