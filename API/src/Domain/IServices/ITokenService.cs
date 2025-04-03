using Domain.Entities;
using System.IdentityModel.Tokens.Jwt;

namespace Domain.IServices;
public interface ITokenService
{
    JwtSecurityToken? CreateToken(User user);
}
