using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace TestSysplan.Core.Infrastructure.Context
{
    public static class ServiceContextExtensions
    {
        private static string GetConnectionString(this IServiceProvider prov, string key = null)
        {            
            IConfiguration configuration = prov.GetService<IConfiguration>();            
            string str;
            if (string.IsNullOrEmpty(key))
            {
                str = configuration
                    ?.GetSection("ConnectionStrings")
                    ?.GetChildren()
                    ?.FirstOrDefault()
                    ?.Value;
            }
            else
            {
                key = key.StartsWith("ConnectionStrings:") ? key : "ConnectionStrings:" + key;
                str = configuration[key];
            }

            return string.IsNullOrWhiteSpace(str) ?
                throw new InvalidOperationException("ConnectionString not set!") :
                str;
        }

        #region LocalContext
        public static IServiceCollection UseLocalContextAsSqlServer(this IServiceCollection services, string configConnectionStringKey = null)
        {
            return services.AddDbContext<LocalContext>((prov, options) =>
                options.UseSqlServer(prov.GetConnectionString(configConnectionStringKey)));
        }

        public static IServiceCollection UseLocalContextAsSqlite(this IServiceCollection services, string configConnectionStringKey = null)
        {
            return services.AddDbContext<LocalContext>((prov, options) =>
                options.UseSqlite(prov.GetConnectionString(configConnectionStringKey)));
        }

        public static IServiceCollection UseLocalContextAsPostgres(this IServiceCollection services, string configConnectionStringKey = null)
        {
            return services
                .AddEntityFrameworkNpgsql()
                .AddDbContext<LocalContext>((prov, options) => 
                    options.UseNpgsql(prov.GetConnectionString(configConnectionStringKey)));
        }
        #endregion
    }
}
