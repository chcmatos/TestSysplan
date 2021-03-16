using Microsoft.Extensions.DependencyInjection;

namespace TestSysplan.Core.Infrastructure.Services
{
    public static class ServiceExtensions
    {
        public static IServiceCollection UseServices(this IServiceCollection services)
        {
            return services.AddScoped<IClientService, ClientService>();
        }

    }
}
