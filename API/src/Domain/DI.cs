using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Domain;
public static class DI
{
    public static IServiceCollection AddDomainLayer(this IServiceCollection services, IConfiguration Config)
    {
      

        return services;
    }
}
