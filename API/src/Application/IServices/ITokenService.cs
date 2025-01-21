using Domain.Models;
using System.IdentityModel.Tokens.Jwt;

namespace Application.IServices;
public interface ITokenService
{
    JwtSecurityToken? CreateToken(User user);
}
