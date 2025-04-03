using Domain.DTO.Request;
using Domain.DTO.Response;

namespace Domain.IServices;
public interface IAuthService
{
    public Task<AuthResponse> Login(LoginDTO model);
    public Task<AuthResponse> Register(LoginDTO model);
}
