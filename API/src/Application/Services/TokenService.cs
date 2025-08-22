using Domain.DTO.Response;
using Domain.Entities;
using Domain.IRepository;
using Domain.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services;
public class TokenService : ITokenService
{
    private readonly JWT _jwt;
    private readonly IUnitOfWork _unitOfWork;
    public TokenService(IOptions<JWT> jwt, IUnitOfWork unitOfWork)
    {
        _jwt = jwt.Value;
        _unitOfWork = unitOfWork;
    }



    public JwtSecurityToken? CreateToken(User user)
    {
        var claims = new[]
        {
          new Claim(ClaimTypes.Surname, user.Username),
          new Claim(ClaimTypes.Hash, Guid.NewGuid().ToString()),
          new Claim(ClaimTypes.NameIdentifier , user.Id.ToString())
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

    public async Task<RefreshTokenDTO> CreateRefreshToken(User user)
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);

            var userId = user.Id.ToString();
            var timestamp = DateTime.UtcNow.Ticks.ToString();

            var tokenData = $"{userId}.{timestamp}.{Convert.ToBase64String(randomNumber)}";

            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(Encoding.UTF8.GetBytes(tokenData)),
                UserId = user.Id,
                ExpireOn = DateTime.UtcNow.AddDays(_jwt.RefreshTokenDurationInDays),
                IsRevoked = false
            };

            await _unitOfWork.RefreshTokens.AddAsync(refreshToken);
            await _unitOfWork.SaveChangesAync();

            return new RefreshTokenDTO{
                RefreshToken = refreshToken.Token,
                RefreshTokenExpireOn = refreshToken.ExpireOn
            };
        }
    }
}

