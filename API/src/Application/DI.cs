using Application.Services;
using Domain.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application;
public static class DI
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services, IConfiguration config)
    {
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
