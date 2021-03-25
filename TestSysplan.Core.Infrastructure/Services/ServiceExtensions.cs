using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace TestSysplan.Core.Infrastructure.Services
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            return services
                .AddSingleton<ILogger>(prov => Serilog.Log.Logger)
                .AddScoped<IClientService, ClientService>()
                .AddScoped<IMessageService, AMQPService>();
        }

    }
}
