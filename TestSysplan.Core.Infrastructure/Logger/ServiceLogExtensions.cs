using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

        public static IServiceCollection AddLogService(this IServiceCollection services)
        {
            return services
                .AddSingleton(prov => Log.Logger);
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
        public static IHostBuilder UseLogServiceWithElasticsearch(this IHostBuilder builder)
        {
            return builder
                .ConfigureServices((hc, services) => services.AddLogServiceWithElasticsearch(hc.Configuration))
                .ConfigureLogging((hc, builder) =>
                    builder.AddSerilog(
                        builder.Services
                            .BuildServiceProvider()
                            .GetRequiredService<Serilog.ILogger>(), dispose: true));
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
        public static IServiceCollection AddLogServiceWithElasticsearch(this IServiceCollection services, IConfiguration configuration = null)
        {
            if(configuration is null)
            {
                var provider  = services.BuildServiceProvider();
                configuration = provider.GetService<IConfiguration>();
            }

            string elasticsearchUrl = ElasticsearchUrl.GetValueOrDefault(configuration?["Elasticsearch:Url"] ?? configuration?["Elasticsearch"]);
            string appName = configuration?["Elasticsearch:Appname"] ?? Assembly.GetEntryAssembly().GetName().Name;

            if (string.IsNullOrWhiteSpace(elasticsearchUrl))
            {
                throw new InvalidOperationException("[Elastisearch:Url|Elasticsearch] URL not set!");
            }

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticsearchUrl))
                {
                    AutoRegisterTemplate = true,
                    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                    IndexFormat = $"{appName}-at-{DateTime.Now:yyyy-MM-dd}",
                    NumberOfReplicas = 1
                }).CreateLogger();

            return services.AddLogService();
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
        public static ILoggerFactory UseLogServiceWithElasticsearch(this ILoggerFactory loggerFactory)
        {
            return loggerFactory.AddSerilog(dispose: true);
        }
    }
}
