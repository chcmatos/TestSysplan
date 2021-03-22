using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TestSysplan.Core.Infrastructure.Context.Environment;

namespace TestSysplan.Core.Infrastructure.Context
{
    public static class ServiceContextExtensions
    {
        #region LocalContext
        public static IServiceCollection AddLocalContextAsSqlServer(this IServiceCollection services, string configConnectionStringKey = null)
        {
            return services.AddDbContext<LocalContext>((prov, options) =>
                options.UseSqlServer(
                    new DatabaseConnectionString.Builder()
                        .UseMsSqlServer()
                        .UseConfiguration(prov)
                        .UseConnectionStringKey(configConnectionStringKey)                        
                        .UseEnvironmentVariables(ConfigEnvironment.LocalContext)
                        .Build()));
        }

        public static IServiceCollection AddLocalContextAsPostgres(this IServiceCollection services, string configConnectionStringKey = null)
        {
            return services
                .AddEntityFrameworkNpgsql()
                .AddDbContext<LocalContext>((prov, options) =>                     
                    options.UseNpgsql(o => o.SetPostgresVersion(9, 6))
                           .UseNpgsql(
                            new DatabaseConnectionString.Builder()
                                .UsePostgres()
                                .UseConfiguration(prov)
                                .UseConnectionStringKey(configConnectionStringKey)
                                .UseEnvironmentVariables(ConfigEnvironment.LocalContext)
                                .Build()));
        }
        #endregion
    }
}
