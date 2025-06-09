namespace Domain.DTO.Response;
public record AuthResponse
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpireOn { get; set; }
    public DateTime RefreshTokenExpireOn { get; set; }
    public bool IsAuth { get; set; }
    public string UserName { get; set;}
    public string Message { get; set; }

}
