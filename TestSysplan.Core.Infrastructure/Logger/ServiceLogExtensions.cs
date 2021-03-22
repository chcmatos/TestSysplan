using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Reflection;
using TestSysplan.Core.Infrastructure.Util;

namespace TestSysplan.Core.Infrastructure.Logger
{
    public static class ServiceLogExtensions
    {
        private static readonly EnvironmentVariable ElasticsearchUrl = "ELASTICSEARCH_URL";

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
        public static IServiceCollection AddLogWithElasticsearch(this IServiceCollection services, IConfiguration configuration = null)
        {
            if(configuration is null)
            {
                var provider  = services.BuildServiceProvider();
                configuration = provider.GetService<IConfiguration>();
            }

            string elasticsearchUrl = ElasticsearchUrl.GetValueOrDefault(configuration["Elasticsearch:Url"] ?? configuration["Elasticsearch"]);
            string appName = configuration["Elasticsearch:Appname"] ?? Assembly.GetEntryAssembly().GetName().Name;

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticsearchUrl))
                {
                    AutoRegisterTemplate = true,
                    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                    IndexFormat = $"{appName}-at-{DateTime.Now:yyyy-MM-dd}",
                    NumberOfReplicas = 1
                })                
                .CreateLogger();

            return services;
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
        public static ILoggerFactory UseLogWithElasticsearch(this ILoggerFactory loggerFactory)
        {
            return loggerFactory.AddSerilog(dispose: true);
        }
    }
}
