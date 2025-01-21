namespace Application.IServices;
public interface IPasswordHasher
{
    string Hash(string password);
    bool IsMatch(string newPassword, string userPassword);
}
