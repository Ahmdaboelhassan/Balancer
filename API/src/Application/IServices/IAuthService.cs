using Application.DTO.Request;
using Application.DTO.Response;

namespace Application.IServices;
public interface IAuthService
{
    public Task<AuthResponse> Login(LoginDTO model);
    public Task<AuthResponse> Register(LoginDTO model);
}
