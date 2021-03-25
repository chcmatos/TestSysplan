using Microsoft.Extensions.DependencyInjection;

namespace TestSysplan.Core.Infrastructure.Services
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            return services
                .AddClientService();
        }

        public static IServiceCollection AddClientService(this IServiceCollection services)
        {
            return services
                .AddScoped<IClientService, ClientService>();
        }

    }
}
