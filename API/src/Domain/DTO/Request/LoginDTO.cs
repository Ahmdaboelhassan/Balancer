namespace Domain.DTO.Request;
public class LoginDTO
{
    public string Username {  get; set; }
    public string Password {  get; set; }
    public bool StayLogin { get; set; }

}
