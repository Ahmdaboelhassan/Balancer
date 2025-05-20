using Domain.DTO.Response;
using Domain.IRepository;
using Domain.IServices;
using Domain.Static;
using Infrastructure.Data;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Infrastructure
{
    public static class DI
    {
        public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration Config)
        {
           string? ConnectionString = Config.GetConnectionString(MagicStrings.Production);
            if (ConnectionString == null)
                throw new InvalidOperationException();

            services.AddDbContext<AppDbContext>(option => option.UseSqlServer(ConnectionString));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.Configure<JWT>(Config.GetSection("JWT"));
            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(JwtOptions =>
            {
                JwtOptions.RequireHttpsMetadata = false;
                JwtOptions.SaveToken = false;
                JwtOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = Config["JWT:Issuer"],
                    ValidAudience = Config["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Config["JWT:Key"]))
                };
            });

            return services;
        }
    }
}
