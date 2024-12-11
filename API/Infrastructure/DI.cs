using Application.IRepository;
using Infrastructure.Data;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DI
    {
        public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration Config)
        {
            string? ConnectionString = Config.GetConnectionString("Primary");
            if (ConnectionString == null)
                throw new InvalidOperationException();

            services.AddDbContext<AppDbContext>(option => option.UseSqlServer(ConnectionString));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
