using Microsoft.AspNetCore.Hosting;
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
        /// <returns></returns>
        public static IWebHostBuilder UseLogWithElasticsearch(this IWebHostBuilder webBuilder)
        {
            return webBuilder.UseSerilog((context, config) =>
            {
                string elasticsearchUrl = context.Configuration["Elasticsearch:Url"] ?? context.Configuration["Elasticsearch"];
                string appName = context.Configuration["Elasticsearch:Appname"] ?? Assembly.GetEntryAssembly().GetName().Name;

                config
                .Enrich.FromLogContext()
#if DEBUG
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .WriteTo.Debug()
                .WriteTo.Console()
#else
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
#endif
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticsearchUrl))
                {
                    AutoRegisterTemplate = true,
                    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                    IndexFormat = $"{appName}-at-{DateTime.Now:yyyy-MM-dd}",
                    NumberOfReplicas = 1
                })                
                .ReadFrom.Configuration(context.Configuration);
            });
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
        /// <returns></returns>
        public static IServiceCollection UseLogWithElasticsearch(this IServiceCollection services)
        {
            return services.AddLogging(builder => builder.AddSerilog(dispose: true));
        }
    }
}
