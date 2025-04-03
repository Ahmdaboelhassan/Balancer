using Domain.DTO.Response;
using Domain.Entities;
using Domain.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services;
public class TokenService : ITokenService
{
    private readonly JWT _jwt;
    public TokenService(IOptions<JWT> jwt)
    {
          _jwt = jwt.Value;
    }
    public JwtSecurityToken? CreateToken(User user)
    {
        var claims = new[]
        {
          new Claim(JwtRegisteredClaimNames.Sub, user.Username),
          new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
          new Claim(JwtRegisteredClaimNames.NameId , user.Id.ToString())
        };

        var jwtKey = _jwt.Key;
        if (string.IsNullOrEmpty(jwtKey))
            return null;

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        return new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_jwt.DurationInMinutes),
            signingCredentials: signingCredentials);
    }
}

