using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Reflection;

namespace TestSysplan.Core.Infrastructure.Logger
{
    public static class ServiceLogExtensions
    {
        public static IWebHostBuilder UseLogWithElasticsearch(this IWebHostBuilder webBuilder)
        {
            return webBuilder.UseSerilog();
        }

        public static IServiceCollection UseLogWithElasticsearch(
            this IServiceCollection services, 
            string elasticsearchUrl, 
            string appName)
        {

            if(string.IsNullOrEmpty(elasticsearchUrl))
            {
                throw new ArgumentNullException(nameof(elasticsearchUrl));
            }
            else if (string.IsNullOrEmpty(appName))
            {
                throw new ArgumentNullException(nameof(appName));
            }

            Serilog.Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
#if DEBUG
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Debug)
                .MinimumLevel.Override("System", LogEventLevel.Debug)
                .WriteTo.Debug()
                .WriteTo.Console()
#else
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Information)
#endif
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticsearchUrl))
                {
                    AutoRegisterTemplate = true,
                    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                    IndexFormat = $"{appName}-at-{DateTime.Now:yyyy-MM-dd}"
                })
                .CreateLogger();

            return services
                .AddLogging(builder => builder.AddSerilog(dispose: true));
        }

        /// <summary>
        /// To use log with elasticsearch inputing configuration 
        /// values, create into <i>appsettings.json</i> project 
        /// the follow field:
        /// <code>
        /// "Elasticsearch": {<br/>
        ///  "Url": "http://localhost:5151/elasticsearch",<br/>
        ///  "Appname":  "YourAppNameInElasticsearch"<br/>
        /// },
        /// </code>
        /// or create the follow simple field within only elasticsearch url,
        /// then the app name will be the current execunting assembly name.
        /// <code>
        /// "Elasticsearch": "http://localhost:5151/elasticsearch"
        /// </code>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection UseLogWithElasticsearch(this IServiceCollection services,
            IConfiguration configuration)
        {
            return services.UseLogWithElasticsearch(
                configuration["Elasticsearch:Url"] ?? configuration["Elasticsearch"],
                configuration["Elasticsearch:Appname"] ?? Assembly.GetEntryAssembly().GetName().Name);
        }
    }
}
