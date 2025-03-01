using Application.IRepository;
using Application.IServices;
using Infrastructure.Data;
using Infrastructure.Repository;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DI
    {
        public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration Config)
        {
            string? ConnectionString = Config.GetConnectionString("Production");
            if (ConnectionString == null)
                throw new InvalidOperationException();

            services.AddDbContext<AppDbContext>(option => option.UseSqlServer(ConnectionString));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPeriodService, PeriodService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAccountNumberService, AccountNumberService>();
            services.AddScoped<IJournalService, JournalService>();
            services.AddScoped<ICostCenterService, CostCenterService>();
            services.AddScoped<IHomeService, HomeService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IServiceContext, ServiceContext>();

            return services;
        }
    }
}
