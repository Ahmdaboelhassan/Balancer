using Domain.DTO.Response;
using Domain.Entities;
using System.IdentityModel.Tokens.Jwt;

namespace Domain.IServices;
public interface ITokenService
{
    JwtSecurityToken? CreateToken(User user);
    Task<RefreshTokenDTO> CreateRefreshToken(User user);
}
