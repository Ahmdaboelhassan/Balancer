namespace Domain.DTO.Response;
public class JWT
{
    public string Key { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int DurationInMinutes { get; set; }
    public int RefreshTokenDurationInDays { get; set; }
    
}
