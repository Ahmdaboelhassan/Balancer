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
        return new AuthResponse
        {
            ExpireOn = token.ValidTo,
            IsAuth = true,
            Message = "Login Successfully",
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            UserName = model.Username,
        };

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

        return new AuthResponse
        {
            ExpireOn = token.ValidTo,
            IsAuth = true,
            Message = "Login Successfully",
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            UserName = model.Username,
        };
    }
}
