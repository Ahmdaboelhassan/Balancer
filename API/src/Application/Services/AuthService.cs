using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.IRepository;
using Domain.IServices;
using Domain.Entities;
using System.IdentityModel.Tokens.Jwt;

namespace Application.Services;
public class AuthService : IAuthService
{
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;
    public AuthService(ITokenService tokenService, IPasswordHasher passwordHasher, IUnitOfWork unitOfWork)
    {
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }


    public async Task<AuthResponse> Login(LoginDTO model)
    {
        var user = await _unitOfWork.Users.Get(u => u.Username == model.Username);

        if (user is null)
            return new AuthResponse() { IsAuth = false, Message = "Invalid Username or Password" };

        var verify = _passwordHasher.IsMatch(model.Password, user.Password);

        if (!verify)
            return new AuthResponse() { IsAuth = false, Message = "Invalid Username or Password" };

        var token = _tokenService.CreateToken(user);

        if (token is null)
            return new AuthResponse() { IsAuth = false, Message = "Something went wrong while creating token" };

        var authResponse = new AuthResponse
        {
            IsAuth = true,
            Message = "Login Successfully",
            UserName = model.Username,
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpireOn = token.ValidTo
        };

        if (model.StayLogin)
        {
            var refreshToken = await _tokenService.CreateRefreshToken(user);
            authResponse.RefreshToken = refreshToken.RefreshToken;
            authResponse.RefreshTokenExpireOn = refreshToken.RefreshTokenExpireOn;
        }

        return authResponse;
    }

    public async Task<AuthResponse> Register(LoginDTO model)
    {
        var user = await _unitOfWork.Users.Get(u => u.Username == model.Username);

        if (user is not null)
            return new AuthResponse() { IsAuth = false, Message = "Invalid Username" };

        var newUser = new User
        {
            Password = _passwordHasher.Hash(model.Password),
            Username = model.Username
        };

        await _unitOfWork.Users.AddAsync(newUser);
        await _unitOfWork.SaveChangesAync();
        
        var token = _tokenService.CreateToken(newUser);

        if (token is null)
            return new AuthResponse() { Message = "Something wrong" };

        var authResponse = new AuthResponse
        {
            IsAuth = true,
            Message = "User Created Successfully",
            UserName = model.Username,
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpireOn = token.ValidTo
        };


        if (model.StayLogin)
        {
            var refreshToken = await _tokenService.CreateRefreshToken(user);
            authResponse.RefreshToken = refreshToken.RefreshToken;
            authResponse.RefreshTokenExpireOn = refreshToken.RefreshTokenExpireOn;
        }

        return authResponse;
    }

    public async Task<AuthResponse> RefreshToken(string? token)
    {
        if (string.IsNullOrEmpty(token))
            return new AuthResponse() {Message = "Invalid Token" };

        var refreshToken = await _unitOfWork.RefreshTokens.Get(rt => rt.Token == token && !rt.IsRevoked, "User");
        if (refreshToken is null)
            return new AuthResponse() {Message = "Invalid Refresh Token" };

        if (refreshToken.ExpireOn < DateTime.UtcNow)
            return new AuthResponse() {Message = "Refresh Token Expired" };

        var user = refreshToken.User;

        var newToken = _tokenService.CreateToken(user);

        if (newToken is null)
            return new AuthResponse() { IsAuth = false, Message = "Something went wrong while creating token" };

        var authResponse = new AuthResponse
        {
            IsAuth = true,
            Message = "Login Successfully",
            UserName = user.Username,
            ExpireOn = newToken.ValidTo,
            Token = new JwtSecurityTokenHandler().WriteToken(newToken),
        };

        if (refreshToken.ExpireOn < DateTime.UtcNow.AddDays(10))
        {
            // delete old refresh token
            _unitOfWork.RefreshTokens.Delete(refreshToken);
            await _unitOfWork.SaveChangesAync();

            // create new refresh token
            var newRefreshToken = await _tokenService.CreateRefreshToken(user);
            authResponse.RefreshToken = newRefreshToken.RefreshToken;
            authResponse.RefreshTokenExpireOn = newRefreshToken.RefreshTokenExpireOn;
        }
        else
        {
            authResponse.RefreshToken = refreshToken.Token;
            authResponse.RefreshTokenExpireOn = refreshToken.ExpireOn;
        }

        return authResponse;
    }
}
