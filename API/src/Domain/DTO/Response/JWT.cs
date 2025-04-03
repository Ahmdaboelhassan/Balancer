namespace Domain.DTO.Response;
public class JWT
{
    public int DurationInMinutes { get; set; }
    public string Key { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    
}
